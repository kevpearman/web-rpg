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
