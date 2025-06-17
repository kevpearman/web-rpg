using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerchantRPG.API.Data;
using MerchantRPG.API.Models;

namespace MerchantRPG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdventurersController : ControllerBase
    {
        private readonly GameDbContext _context;

        public AdventurersController(GameDbContext context)
        {
            _context = context;
        }

        // GET: api/adventurers/hiring-tiers
        [HttpGet("hiring-tiers")]
        public ActionResult<object> GetHiringTiers()
        {
            var tiers = new[]
            {
                new {
                    Tier = "rookie",
                    Name = "Rookie",
                    Cost = 100,
                    Stats = new { Strength = 8, Agility = 8, Intelligence = 8, Speed = 4 },
                    Description = "A basic adventurer ready for simple missions"
                },
                new {
                    Tier = "veteran",
                    Name = "Veteran",
                    Cost = 250,
                    Stats = new { Strength = 12, Agility = 12, Intelligence = 12, Speed = 6 },
                    Description = "An experienced adventurer with proven skills"
                },
                new {
                    Tier = "elite",
                    Name = "Elite",
                    Cost = 500,
                    Stats = new { Strength = 16, Agility = 16, Intelligence = 16, Speed = 8 },
                    Description = "A top-tier adventurer capable of the toughest missions"
                }
            };

            return Ok(tiers);
        }

        // POST: api/adventurers
        [HttpPost]
        public async Task<ActionResult<HireAdventurerResponse>> HireAdventurer([FromBody] HireAdventurerRequest request)
        {
            try
            {
                var player = await _context.Players
                    .Include(p => p.Adventurers)
                    .FirstOrDefaultAsync(p => p.Id == request.PlayerId);

                if (player == null)
                {
                    return BadRequest(new HireAdventurerResponse
                    {
                        Success = false,
                        Message = "Player not found"
                    });
                }

                // Check adventurer cap (2 maximum)
                if (player.Adventurers?.Count >= 2)
                {
                    return BadRequest(new HireAdventurerResponse
                    {
                        Success = false,
                        Message = "Maximum number of adventurers reached (2)"
                    });
                }

                // Define adventurer tiers with costs and stats
                var (cost, stats, name) = request.AdventurerTier.ToLower() switch
                {
                    "rookie" => (100, new { Str = 8, Agi = 8, Int = 8, Speed = 4 }, "Rookie"),
                    "veteran" => (250, new { Str = 12, Agi = 12, Int = 12, Speed = 6 }, "Veteran"),
                    "elite" => (500, new { Str = 16, Agi = 16, Int = 16, Speed = 8 }, "Elite"),
                    _ => throw new ArgumentException("Invalid adventurer tier")
                };

                // Check if player has enough gold
                if (player.Gold < cost)
                {
                    return BadRequest(new HireAdventurerResponse
                    {
                        Success = false,
                        Message = $"Not enough gold. Need {cost}g, have {player.Gold}g"
                    });
                }

                // Generate adventurer name
                var adventurerNames = new[]
                {
                    "Gareth", "Luna", "Marcus", "Aria", "Thorne", "Zara", "Darius", "Nova",
                    "Kael", "Raven", "Felix", "Ivy", "Atlas", "Sage", "Orion", "Maya"
                };
                var randomName = adventurerNames[new Random().Next(adventurerNames.Length)];
                var fullName = $"{randomName} the {name}";

                // Create new adventurer
                var newAdventurer = new Adventurer
                {
                    PlayerId = player.Id,
                    Name = fullName,
                    Level = 1,
                    Strength = stats.Str,
                    Agility = stats.Agi,
                    Intelligence = stats.Int,
                    Speed = stats.Speed,
                    Experience = 0,
                    IsOnMission = false,
                    CreatedAt = DateTime.UtcNow
                };

                // Deduct gold and add adventurer
                player.Gold -= cost;
                _context.Adventurers.Add(newAdventurer);

                await _context.SaveChangesAsync();

                return Ok(new HireAdventurerResponse
                {
                    Success = true,
                    Message = $"Successfully hired {fullName}!",
                    NewAdventurer = newAdventurer,
                    RemainingGold = player.Gold
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new HireAdventurerResponse
                {
                    Success = false,
                    Message = $"Error hiring adventurer: {ex.Message}"
                });
            }
        }
    }

    // Request/Response classes
    public class HireAdventurerRequest
    {
        public int PlayerId { get; set; }
        public string AdventurerTier { get; set; } = string.Empty;
    }

    public class HireAdventurerResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Adventurer? NewAdventurer { get; set; }
        public int RemainingGold { get; set; }
    }
}