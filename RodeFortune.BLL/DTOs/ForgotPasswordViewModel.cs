using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "Електронна пошта обов'язкова")]
        [EmailAddress(ErrorMessage = "Невірний формат електронної пошти")]
        [Display(Name = "Електронна пошта")]
        public string Email { get; set; }
    }
}