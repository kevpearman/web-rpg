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
