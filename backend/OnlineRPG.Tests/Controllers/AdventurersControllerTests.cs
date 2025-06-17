using Microsoft.AspNetCore.Mvc;
using MerchantRPG.API.Controllers;
using OnlineRPG.Tests.Infrastructure;
using Xunit;
using MerchantRPG.API.Data;

namespace OnlineRPG.Tests.Controllers
{
    public class AdventurersControllerTests : IDisposable
    {
        private readonly AdventurersController _controller;
        private readonly GameDbContext _context;

        public AdventurersControllerTests()
        {
            _context = TestDbContextFactory.CreateInMemoryContext();
            _controller = new AdventurersController(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public void GetHiringTiers_ReturnsCorrectTiers()
        {
            // Act
            var result = _controller.GetHiringTiers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var tiers = Assert.IsAssignableFrom<object[]>(okResult.Value);
            Assert.Equal(3, tiers.Length);
        }

        [Fact]
        public async Task HireAdventurer_WithValidRequest_ReturnsSuccess()
        {
            var playerId = 1;
            var startingGold = _context.Players.First(_ => _.Id == playerId).Gold;
            var rookieAdventurerCost = 100;

            // Arrange
            var request = new HireAdventurerRequest
            {
                PlayerId = playerId,
                AdventurerTier = "rookie"
            };

            // Act
            var result = await _controller.HireAdventurer(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<HireAdventurerResponse>(okResult.Value);
            
            Assert.True(response.Success);
            Assert.Contains("Successfully hired", response.Message);
            Assert.NotNull(response.NewAdventurer);
            Assert.Equal(startingGold - rookieAdventurerCost, response.RemainingGold);
            Assert.Equal(8, response.NewAdventurer.Strength);
            Assert.Equal(4, response.NewAdventurer.Speed);
        }

        [Fact]
        public async Task HireAdventurer_WithInsufficientGold_ReturnsBadRequest()
        {
            // Arrange
            var request = new HireAdventurerRequest
            {
                PlayerId = 3, // PoorPlayer with 50 gold
                AdventurerTier = "rookie" // Costs 100 gold
            };

            // Act
            var result = await _controller.HireAdventurer(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<HireAdventurerResponse>(badRequestResult.Value);
            
            Assert.False(response.Success);
            Assert.Contains("Not enough gold", response.Message);
        }

        [Fact]
        public async Task HireAdventurer_WhenAtCapacity_ReturnsBadRequest()
        {
            // Arrange - Player 2 already has 2 adventurers
            var request = new HireAdventurerRequest
            {
                PlayerId = 2, // RichPlayer with 2 existing adventurers
                AdventurerTier = "rookie"
            };

            // Act
            var result = await _controller.HireAdventurer(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<HireAdventurerResponse>(badRequestResult.Value);
            
            Assert.False(response.Success);
            Assert.Contains("Maximum number of adventurers reached", response.Message);
        }

        [Fact]
        public async Task HireAdventurer_WithInvalidTier_ReturnsBadRequest()
        {
            // Arrange
            var request = new HireAdventurerRequest
            {
                PlayerId = 1,
                AdventurerTier = "invalid_tier"
            };

            // Act
            var result = await _controller.HireAdventurer(request);

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
            
            var response = Assert.IsType<HireAdventurerResponse>(statusResult.Value);
            Assert.False(response.Success);
            Assert.Contains("Error hiring adventurer", response.Message);
        }

        [Fact]
        public async Task HireAdventurer_WithNonexistentPlayer_ReturnsBadRequest()
        {
            // Arrange
            var request = new HireAdventurerRequest
            {
                PlayerId = 999, // Non-existent player
                AdventurerTier = "rookie"
            };

            // Act
            var result = await _controller.HireAdventurer(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<HireAdventurerResponse>(badRequestResult.Value);
            
            Assert.False(response.Success);
            Assert.Contains("Player not found", response.Message);
        }

        [Theory]
        [InlineData("rookie", 100, 8, 4)]
        [InlineData("veteran", 250, 12, 6)]
        [InlineData("elite", 500, 16, 8)]
        public async Task HireAdventurer_WithDifferentTiers_ReturnsCorrectStats(
            string tier, int expectedCost, int expectedStr, int expectedSpeed)
        {
            // Arrange
            var request = new HireAdventurerRequest
            {
                PlayerId = 2, // RichPlayer with 1000 gold (delete existing adventurers first)
                AdventurerTier = tier
            };

            // Remove existing adventurers for this test
            var existingAdventurers = _context.Adventurers.Where(a => a.PlayerId == 2).ToList();
            _context.Adventurers.RemoveRange(existingAdventurers);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.HireAdventurer(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var response = Assert.IsType<HireAdventurerResponse>(okResult.Value);
            
            Assert.True(response.Success);
            Assert.Equal(1000 - expectedCost, response.RemainingGold);
            Assert.Equal(expectedStr, response.NewAdventurer.Strength);
            Assert.Equal(expectedSpeed, response.NewAdventurer.Speed);
            Assert.Contains(tier.Substring(0, 1).ToUpper() + tier.Substring(1), response.NewAdventurer.Name);
        }

        [Fact]
        public async Task HireAdventurer_CreatesAdventurerInDatabase()
        {
            // Arrange
            var initialCount = _context.Adventurers.Count();
            var request = new HireAdventurerRequest
            {
                PlayerId = 1,
                AdventurerTier = "veteran"
            };

            // Act
            var response = await _controller.HireAdventurer(request);

            // Assert
            var finalCount = _context.Adventurers.Count();
            Assert.Equal(initialCount + 1, finalCount);
            
            var newAdventurer = _context.Adventurers.OrderBy(a => a.Id).Last();
            Assert.Equal(1, newAdventurer.PlayerId);
            Assert.Equal(12, newAdventurer.Strength);
            Assert.Contains("Veteran", newAdventurer.Name);
        }

        [Fact]
        public async Task HireAdventurer_UpdatesPlayerGold()
        {
            // Arrange
            var player = _context.Players.First(p => p.Id == 1);
            var initialGold = player.Gold;
            
            var request = new HireAdventurerRequest
            {
                PlayerId = 1,
                AdventurerTier = "rookie"
            };

            // Act
            await _controller.HireAdventurer(request);

            // Assert
            _context.Entry(player).Reload(); // Refresh from database
            Assert.Equal(initialGold - 100, player.Gold);
        }
    }
}