using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Ім'я користувача обов'язкове")]
        [Display(Name = "Ім'я користувача")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Електронна пошта обов'язкова")]
        [EmailAddress(ErrorMessage = "Невірний формат електронної пошти")]
        [Display(Name = "Електронна пошта")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обов'язковий")]
        [MinLength(8, ErrorMessage = "Пароль повинен містити не менше 8 символів")]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Дата народження обов'язкова")]
        [Display(Name = "Дата народження")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
    }
}