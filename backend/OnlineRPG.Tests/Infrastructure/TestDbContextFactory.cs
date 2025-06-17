using Microsoft.EntityFrameworkCore;
using MerchantRPG.API.Data;
using MerchantRPG.API.Models;

namespace OnlineRPG.Tests.Infrastructure
{
    public class TestDbContextFactory
    {
        public static GameDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<GameDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new GameDbContext(options);

            // Seed test data
            SeedTestData(context);

            return context;
        }

        private static void SeedTestData(GameDbContext context)
        {
            // Add test players
            var testPlayers = new[]
            {
                new Player
                {
                    Id = 1,
                    Username = "TestPlayer1",
                    Email = "test1@example.com",
                    PasswordHash = "hash",
                    Gold = 300,
                    CreatedAt = DateTime.UtcNow,
                    LastActive = DateTime.UtcNow
                },
                new Player
                {
                    Id = 2,
                    Username = "RichPlayer",
                    Email = "rich@example.com",
                    PasswordHash = "hash",
                    Gold = 1000,
                    CreatedAt = DateTime.UtcNow,
                    LastActive = DateTime.UtcNow
                },
                new Player
                {
                    Id = 3,
                    Username = "PoorPlayer",
                    Email = "poor@example.com",
                    PasswordHash = "hash",
                    Gold = 50,
                    CreatedAt = DateTime.UtcNow,
                    LastActive = DateTime.UtcNow
                }
            };

            context.Players.AddRange(testPlayers);

            // Add existing adventurers for testing caps
            var existingAdventurers = new[]
            {
                new Adventurer
                {
                    Id = 1,
                    PlayerId = 2,
                    Name = "Existing Adventurer 1",
                    Level = 1,
                    Strength = 10,
                    Agility = 10,
                    Intelligence = 10,
                    Speed = 5,
                    Experience = 0,
                    IsOnMission = false,
                    CreatedAt = DateTime.UtcNow
                },
                new Adventurer
                {
                    Id = 2,
                    PlayerId = 2,
                    Name = "Existing Adventurer 2",
                    Level = 1,
                    Strength = 10,
                    Agility = 10,
                    Intelligence = 10,
                    Speed = 5,
                    Experience = 0,
                    IsOnMission = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            context.Adventurers.AddRange(existingAdventurers);

            // Add test game maps
            var testMaps = new[]
            {
                new GameMap
                {
                    Id = "forest_clearing",
                    Name = "Forest Clearing",
                    Difficulty = "easy",
                    RequiredLevel = 1,
                    Duration = 300,
                    Distance = 50,
                    BaseCompletionTime = 180,
                    RewardRanges = "{}",
                    UnlocksMapId = "goblin_camp"
                }
            };

            context.GameMaps.AddRange(testMaps);
            context.SaveChanges();
        }
    }
}