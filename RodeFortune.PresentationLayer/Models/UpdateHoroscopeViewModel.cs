using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
    public class UpdateHoroscopeViewModel
    {
        public string Id { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Знак зодіаку не може бути довшим за 50 символів")]
        public string ZodiacSign { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Девіз не може бути довшим за 200 символів")]
        public string Motto { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Зміст гороскопу не може бути довшим за 1000 символів")]
        public string Content { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
