using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
       public class CommentViewModel
        {
            public string Id { get; set; }

            [Required(ErrorMessage = "Коментар не може бути порожнім")]
            [StringLength(1000, ErrorMessage = "Коментар не може перевищувати 1000 символів")]
            [Display(Name = "Ваш коментар")]
            public string Content { get; set; }

            public string AuthorName { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? UpdatedAt { get; set; }
            public bool IsAuthor { get; set; }
            public string PostId { get; set; }
            public bool IsEdited => UpdatedAt.HasValue && UpdatedAt.Value != CreatedAt;
        }
}