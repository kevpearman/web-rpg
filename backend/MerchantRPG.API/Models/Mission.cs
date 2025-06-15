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
