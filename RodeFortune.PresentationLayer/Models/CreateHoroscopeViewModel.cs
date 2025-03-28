using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
    public class CreateHoroscopeViewModel
    {

        public string Id { get; set; } = string.Empty;

        [Required(ErrorMessage = "Знак зодіаку є обов'язковим")]
        [StringLength(50, ErrorMessage = "Знак зодіаку не може бути довшим за 50 символів")]
        public string ZodiacSign { get; set; }

        [StringLength(200, ErrorMessage = "Девіз не може бути довшим за 200 символів")]
        public string Motto { get; set; }

        [Required(ErrorMessage = "Зміст гороскопу є обов'язковим")]
        [StringLength(1000, ErrorMessage = "Зміст гороскопу не може бути довшим за 1000 символів")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Дата є обов'язковою")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
