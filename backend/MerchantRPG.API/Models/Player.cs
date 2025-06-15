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
