using System.ComponentModel.DataAnnotations;

namespace MerchantRPG.API.Models
{
    public class Adventurer
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Level { get; set; }
        public int Strength { get; set; }
        public int Agility { get; set; }
        public int Intelligence { get; set; }
        public int Experience { get; set; }
        public bool IsOnMission { get; set; }
        public int Speed { get; set; } // NEW
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public Player? Player { get; set; }
    }
}
