// Controllers/PlayersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerchantRPG.API.Data;
using MerchantRPG.API.Models;

namespace MerchantRPG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PlayersController : ControllerBase
    {
        private readonly GameDbContext _context;

        public PlayersController(GameDbContext context)
        {
            _context = context;
        }

        // GET: api/players
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetPlayers()
        {
            var players = await _context.Players
                .Include(p => p.Adventurers)
                .Include(p => p.Resources)
                .Select(p => new
                {
                    p.Id,
                    p.Username,
                    p.Gold,
                    p.CreatedAt,
                    p.LastActive,
                    AdventurerCount = p.Adventurers.Count,
                    ResourceCount = p.Resources.Count
                })
                .ToListAsync();

            return Ok(players);
        }

        // GET: api/players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetPlayer(int id)
        {
            var player = await _context.Players
                .Include(p => p.Adventurers)
                .Include(p => p.Resources)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
            {
                return NotFound();
            }

            return Ok(new
            {
                player.Id,
                player.Username,
                player.Email,
                player.Gold,
                player.CreatedAt,
                player.LastActive,
                Adventurers = player.Adventurers.Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.Level,
                    a.Strength,
                    a.Agility,
                    a.Intelligence,
                    a.Experience,
                    a.IsOnMission
                }),
                Resources = player.Resources.Select(r => new
                {
                    r.ResourceType,
                    r.Quantity
                })
            });
        }

        // POST: api/players
        [HttpPost]
        public async Task<ActionResult<object>> CreatePlayer(CreatePlayerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if username or email already exists
            if (await _context.Players.AnyAsync(p => p.Username == request.Username || p.Email == request.Email))
            {
                return Conflict("Username or email already exists");
            }

            var player = new Player
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Gold = 100
            };

            // Create starting adventurer
            var startingAdventurer = new Adventurer
            {
                Name = $"{request.Username}'s Companion",
                Player = player
            };

            _context.Players.Add(player);
            _context.Adventurers.Add(startingAdventurer);

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, new
            {
                player.Id,
                player.Username,
                player.Gold,
                Message = "Player created successfully with starting adventurer!"
            });
        }

        // GET: api/players/test-data
        [HttpPost("test-data")]
        public async Task<ActionResult<object>> CreateTestData()
        {
            // Create a test player for development
            var testPlayer = new Player
            {
                Username = "TestMerchant",
                Email = "test@merchant.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                Gold = 500
            };

            var testAdventurer = new Adventurer
            {
                Name = "Test Hero",
                Level = 3,
                Strength = 15,
                Agility = 12,
                Intelligence = 10,
                Experience = 250,
                Player = testPlayer
            };

            var testResources = new List<PlayerResource>
            {
                new() { ResourceType = "wood", Quantity = 10, Player = testPlayer },
                new() { ResourceType = "iron_ore", Quantity = 5, Player = testPlayer },
                new() { ResourceType = "leather", Quantity = 3, Player = testPlayer }
            };

            _context.Players.Add(testPlayer);
            _context.Adventurers.Add(testAdventurer);
            _context.PlayerResources.AddRange(testResources);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Test data created successfully!",
                PlayerId = testPlayer.Id,
                testPlayer.Username,
                testPlayer.Gold,
                AdventurerName = testAdventurer.Name,
                ResourceCount = testResources.Count
            });
        }
    }

    // Request DTOs
    public class CreatePlayerRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}