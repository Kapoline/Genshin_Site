using System.ComponentModel.DataAnnotations;

namespace GenshinPomoyka.Models
{
    public class RegRequest
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "wrong format of email")]
        public string Email { get; set; }
        
        [Required]
        [RegularExpression(@"^(?=.{8,16}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$", ErrorMessage = "минимальная длина 8 карактеров и максимальная 16 по крайней мере одна цифра, нижний регистр и один верхний регистр")]
        public string Password { get; set; }
        
        [Required]
        public string Nickname { get; set; }
        
        [Required]
        [Compare("Password", ErrorMessage = "Пароли различаются")]
        public string ConfirmPassword { get; set; }
        
    }
}