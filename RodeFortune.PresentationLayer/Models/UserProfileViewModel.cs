using System;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
    public class UserProfileViewModel
    {
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ім'я користувача є обов'язковим")]
        [StringLength(50, ErrorMessage = "Ім'я користувача не може бути довшим за 50 символів")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email є обов'язковим")]
        [EmailAddress(ErrorMessage = "Некоректний формат Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Дата народження є обов'язковою")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } 

        public string ZodiacSign { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string Base64Avatar { get; set; } = string.Empty;
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}