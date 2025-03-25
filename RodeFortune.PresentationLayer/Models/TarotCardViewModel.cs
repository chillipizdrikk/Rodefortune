using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
    public class TarotCardViewModel
    {

        [Required(ErrorMessage = "Назва карти обов'язкова")]
        [StringLength(50, ErrorMessage = "Назва повинна бути від {2} до {1} символів", MinimumLength = 2)]
        [Display(Name = "Назва")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Опис карти обов'язковий")]
        [StringLength(1000, ErrorMessage = "Опис повинен бути від {2} до {1} символів", MinimumLength = 10)]
        [Display(Name = "Опис")]
        public string Motto { get; set; }

        [Required(ErrorMessage = "Аркан обов'язковий")]
        public string Arcana { get; set; }

        [Required(ErrorMessage = "Значення карти обов'язкове")]
        [StringLength(500, ErrorMessage = "Значення повинно бути від {2} до {1} символів", MinimumLength = 10)]
        [Display(Name = "Значення")]
        public string Meaning { get; set; }

        public bool Reversal { get; set; }

    
    }
}
