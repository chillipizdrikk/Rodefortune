using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
    public class TarotCardViewModel
    {

        [StringLength(50, ErrorMessage = "Назва повинна бути від {2} до {1} символів", MinimumLength = 2)]
        [Display(Name = "Назва")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Опис повинен бути від {2} до {1} символів", MinimumLength = 10)]
        [Display(Name = "Опис")]
        public string Motto { get; set; } = string.Empty;

        public string Arcana { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Значення повинно бути від {2} до {1} символів", MinimumLength = 10)]
        [Display(Name = "Значення")]
        public string Meaning { get; set; } = string.Empty;

        public bool Reversal { get; set; }

    
    }
}
