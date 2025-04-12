using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Поточний пароль обов'язковий")]
        [DataType(DataType.Password)]
        [Display(Name = "Поточний пароль")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Новий пароль обов'язковий")]
        [StringLength(100, ErrorMessage = "Пароль повинен бути довжиною не менше {2} символів.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Новий пароль")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Підтвердження пароля обов'язкове")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        [Compare("NewPassword", ErrorMessage = "Пароль і підтвердження пароля не співпадають.")]
        public string ConfirmPassword { get; set; }
    }
}