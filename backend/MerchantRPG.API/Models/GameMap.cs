using System.ComponentModel.DataAnnotations;

namespace MerchantRPG.API.Models
{
    public class GameMap
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
        public int RequiredLevel { get; set; }
        public int Duration { get; set; } // Keep this for backwards compatibility
        public int Distance { get; set; } // NEW
        public int BaseCompletionTime { get; set; } // NEW
        public string RewardRanges { get; set; } = string.Empty; // JSON
        public string? UnlocksMapId { get; set; }
    }
}
