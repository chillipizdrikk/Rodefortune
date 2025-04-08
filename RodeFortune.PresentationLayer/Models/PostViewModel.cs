using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
    public class PostViewModel
    {
        public string? Id { get; set; }

        [Display(Name = "Заголовок допису")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Текст допису")] 
        public string Content { get; set; } =  string.Empty ;

        public DateTime? CreatedAt { get; set; }
        public ObjectId? Author { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "Зображення")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Зображення")]
        public byte[]? ImageData { get; set; }

        [Display(Name = "Пов'язаний розклад")]
        public string? ReferencedReadingId { get; set; }

        [Display(Name = "Пов'язаний гороскоп")]
        public string? ReferencedHoroscopeId { get; set; }

        [Display(Name = "Пов'язана натальна карта")]
        public string? ReferencedNatalChartId { get; set; }

        [Display(Name = "Пов'язана матриця долі")]
        public string? ReferencedDestinyMatrixId { get; set; }

    }
}
