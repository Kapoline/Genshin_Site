using System.ComponentModel.DataAnnotations;

namespace GenshinPomoyka.Models
{
    public class AuthRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}