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
