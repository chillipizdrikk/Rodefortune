using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
    public class PostCreateViewModel
    {
        [Required(ErrorMessage = "Необхідно вказати назву допису")]
        [Display(Name = "Заголовок допису")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Необхідно додати текст допису")]
        [Display(Name = "Текст допису")]
        public string Content { get; set; }
        public string ReferencedReadingId { get; set; }
        public string ReferencedHoroscopeId { get; set; }
        public string ReferencedNatalChartId { get; set; }
        public string ReferencedDestinyMatrixId { get; set; }
    }
}
