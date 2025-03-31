using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string Token { get; set; }
        
        [Required(ErrorMessage = "Пароль обов'язковий")]
        [StringLength(100, ErrorMessage = "Пароль повинен бути довжиною не менше {2} символів.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("Password", ErrorMessage = "Пароль і підтвердження пароля не співпадають.")]
        public string ConfirmPassword { get; set; }
    }
}