using System.Reflection;
using MerchantRPG.API.Models;
using Microsoft.EntityFrameworkCore;

namespace MerchantRPG.API.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }

        public DbSet<Player> Players { get; set; }
        public DbSet<Adventurer> Adventurers { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<PlayerResource> PlayerResources { get; set; }
        public DbSet<GameMap> GameMaps { get; set; }
        public DbSet<MissionHistory> MissionHistory { get; set; } // NEW

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Your existing configurations...

            // Add configuration for MissionHistory
            modelBuilder.Entity<MissionHistory>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasDefaultValue("active");
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Foreign key relationships
                entity.HasOne(e => e.Player)
                      .WithMany()
                      .HasForeignKey(e => e.PlayerId);

                entity.HasOne(e => e.Adventurer)
                      .WithMany()
                      .HasForeignKey(e => e.AdventurerId);

                entity.HasOne(e => e.GameMap)
                      .WithMany()
                      .HasForeignKey(e => e.MapId);
            });
        }
    }
}
