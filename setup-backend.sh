#!/bin/bash

# Merchant RPG Backend Setup Script
# Run this from your project root directory

echo "Setting up Merchant RPG Backend..."

# Navigate to backend folder and create project
cd backend
dotnet new webapi -n MerchantRPG.API
cd MerchantRPG.API

# Add required packages
echo "Adding NuGet packages..."
dotnet add package Microsoft.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.AspNetCore.SignalR
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package BCrypt.Net-Next

# Create directory structure
mkdir -p Models Data Controllers

# Create Models/Player.cs
cat > Models/Player.cs << 'EOF'
using System.ComponentModel.DataAnnotations;

namespace MerchantRPG.API.Models
{
    public class Player
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        
        public int Gold { get; set; } = 100;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public List<Adventurer> Adventurers { get; set; } = new();
        public List<PlayerResource> Resources { get; set; } = new();
    }
}
EOF

# Create Models/Adventurer.cs
cat > Models/Adventurer.cs << 'EOF'
using System.ComponentModel.DataAnnotations;

namespace MerchantRPG.API.Models
{
    public class Adventurer
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        public int Level { get; set; } = 1;
        public int Strength { get; set; } = 10;
        public int Agility { get; set; } = 10;
        public int Intelligence { get; set; } = 10;
        public int Experience { get; set; } = 0;
        
        public bool IsOnMission { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Player Player { get; set; } = null!;
        public List<Mission> Missions { get; set; } = new();
    }
}
EOF

# Create Models/Mission.cs
cat > Models/Mission.cs << 'EOF'
using System.ComponentModel.DataAnnotations;

namespace MerchantRPG.API.Models
{
    public class Mission
    {
        public int Id { get; set; }
        public int AdventurerId { get; set; }
        
        [Required]
        public string MapId { get; set; } = string.Empty;
        
        public DateTime StartTime { get; set; }
        public int Duration { get; set; } // in seconds
        
        [StringLength(20)]
        public string Status { get; set; } = "active"; // active, completed, failed
        
        public string? Rewards { get; set; } // JSON string for flexibility
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public Adventurer Adventurer { get; set; } = null!;
        
        // Computed property
        public DateTime EndTime => StartTime.AddSeconds(Duration);
        public bool IsCompleted => DateTime.UtcNow >= EndTime;
    }
}
EOF

# Create Models/PlayerResource.cs
cat > Models/PlayerResource.cs << 'EOF'
using System.ComponentModel.DataAnnotations;

namespace MerchantRPG.API.Models
{
    public class PlayerResource
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string ResourceType { get; set; } = string.Empty;
        
        public int Quantity { get; set; } = 0;
        
        // Navigation properties
        public Player Player { get; set; } = null!;
    }
}
EOF

# Create Models/GameMap.cs
cat > Models/GameMap.cs << 'EOF'
using System.ComponentModel.DataAnnotations;

namespace MerchantRPG.API.Models
{
    public class GameMap
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(20)]
        public string Difficulty { get; set; } = "easy";
        
        public int RequiredLevel { get; set; } = 1;
        public int Duration { get; set; } // in seconds
        
        public string RewardRanges { get; set; } = "{}"; // JSON string
        
        public string? UnlocksMapId { get; set; }
        
        // Navigation properties
        public GameMap? UnlocksMap { get; set; }
    }
}
EOF

# Create Data/GameDbContext.cs
cat > Data/GameDbContext.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using MerchantRPG.API.Models;

namespace MerchantRPG.API.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Adventurer> Adventurers { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<PlayerResource> PlayerResources { get; set; }
        public DbSet<GameMap> GameMaps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Player configuration
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.HasMany(e => e.Adventurers)
                    .WithOne(e => e.Player)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                entity.HasMany(e => e.Resources)
                    .WithOne(e => e.Player)
                    .HasForeignKey(e => e.PlayerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Adventurer configuration
            modelBuilder.Entity<Adventurer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.PlayerId, e.Name });
                
                entity.HasMany(e => e.Missions)
                    .WithOne(e => e.Adventurer)
                    .HasForeignKey(e => e.AdventurerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Mission configuration
            modelBuilder.Entity<Mission>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.AdventurerId, e.Status });
                entity.HasIndex(e => e.StartTime);
            });

            // PlayerResource configuration
            modelBuilder.Entity<PlayerResource>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.PlayerId, e.ResourceType }).IsUnique();
            });

            // GameMap configuration
            modelBuilder.Entity<GameMap>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.HasOne(e => e.UnlocksMap)
                    .WithMany()
                    .HasForeignKey(e => e.UnlocksMapId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Seed initial game maps
            SeedGameMaps(modelBuilder);
        }

        private static void SeedGameMaps(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameMap>().HasData(
                new GameMap
                {
                    Id = "forest_clearing",
                    Name = "Forest Clearing",
                    Difficulty = "easy",
                    RequiredLevel = 1,
                    Duration = 300, // 5 minutes
                    RewardRanges = """{"gold": {"min": 10, "max": 25}, "materials": [{"type": "wood", "min": 1, "max": 3}]}""",
                    UnlocksMapId = "goblin_camp"
                },
                new GameMap
                {
                    Id = "goblin_camp",
                    Name = "Goblin Camp",
                    Difficulty = "easy",
                    RequiredLevel = 2,
                    Duration = 600, // 10 minutes
                    RewardRanges = """{"gold": {"min": 20, "max": 40}, "materials": [{"type": "iron_ore", "min": 1, "max": 2}, {"type": "leather", "min": 1, "max": 2}]}""",
                    UnlocksMapId = "dark_cave"
                },
                new GameMap
                {
                    Id = "dark_cave",
                    Name = "Dark Cave",
                    Difficulty = "medium",
                    RequiredLevel = 5,
                    Duration = 900, // 15 minutes
                    RewardRanges = """{"gold": {"min": 50, "max": 80}, "materials": [{"type": "gems", "min": 1, "max": 2}, {"type": "rare_metals", "min": 1, "max": 1}]}"""
                }
            );
        }
    }
}
EOF

# Create Controllers/PlayersController.cs
cat > Controllers/PlayersController.cs << 'EOF'
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
EOF

# Update Program.cs
cat > Program.cs << 'EOF'
using Microsoft.EntityFrameworkCore;
using MerchantRPG.API.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(connectionString))
{
    // Railway provides DATABASE_URL in a format that needs to be converted
    if (connectionString.StartsWith("postgresql://"))
    {
        // Convert Railway's DATABASE_URL format to Entity Framework format
        var uri = new Uri(connectionString);
        var npgsqlConnectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
        connectionString = npgsqlConnectionString;
    }
    
    builder.Services.AddDbContext<GameDbContext>(options =>
        options.UseNpgsql(connectionString));
}
else
{
    // Fallback for local development
    builder.Services.AddDbContext<GameDbContext>(options =>
        options.UseNpgsql("Host=localhost;Database=merchant_rpg;Username=postgres;Password=password"));
}

// CORS configuration for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://*.vercel.app")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// SignalR for real-time updates
builder.Services.AddSignalR();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Add a health check endpoint
app.MapGet("/health", () => "Healthy");

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    try
    {
        context.Database.EnsureCreated();
        Console.WriteLine("Database initialized successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization failed: {ex.Message}");
    }
}

app.Run();
EOF

echo "Backend files created successfully!"
echo ""
echo "Next steps:"
echo "1. Install Railway CLI: npm install -g @railway/cli"
echo "2. Login to Railway: railway login"
echo "3. Create new project: railway new merchant-rpg-backend"
echo "4. Add PostgreSQL: railway add postgresql"
echo "5. Build project: dotnet build"
echo "6. Deploy: railway up"
echo ""
echo "Then commit and push to GitHub:"
echo "git add ."
echo "git commit -m 'Add complete backend setup'"
echo "git push origin main"