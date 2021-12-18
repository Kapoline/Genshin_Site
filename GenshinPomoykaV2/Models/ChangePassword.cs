using System.ComponentModel.DataAnnotations;

namespace GenshinPomoykaV2.Models
{
    public class ChangePassword
    {
        [Required]
        [RegularExpression(@"^(?=.{8,16}$)(?=.*?[a-z])(?=.*?[A-Z])(?=.*?[0-9]).*$", ErrorMessage = "минимальная длина 8 карактеров и максимальная 16 по крайней мере одна цифра, нижний регистр и один верхний регистр")]
        public string Password { get; set; }
        
        [Required]
        [Compare("Password", ErrorMessage = "Пароли различаются")]
        public string ConfirmPassword { get; set; }
    }
}