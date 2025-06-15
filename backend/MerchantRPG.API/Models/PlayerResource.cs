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
