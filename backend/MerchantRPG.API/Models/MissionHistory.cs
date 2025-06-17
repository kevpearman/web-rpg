using System.ComponentModel.DataAnnotations.Schema;

namespace MerchantRPG.API.Models
{
    public class MissionHistory
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int AdventurerId { get; set; }
        public string MapId { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EstimatedEndTime { get; set; }
        public DateTime? ActualEndTime { get; set; }
        public string Status { get; set; } = "active"; // active, completed, collected
        public int TotalDuration { get; set; } // in seconds
        public string MissionSteps { get; set; } = string.Empty; // JSON
        public string? Rewards { get; set; } // JSON
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Player? Player { get; set; }
        public Adventurer? Adventurer { get; set; }
        public GameMap? GameMap { get; set; }
    }
}