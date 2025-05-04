using System.ComponentModel.DataAnnotations;

namespace RodeFortune.PresentationLayer.Models
{
    public class EditCommentViewModel
    {
        public string CommentId { get; set; }
        public string PostId { get; set; }
        
        [Required(ErrorMessage = "Вміст коментаря обов'язковий")]
        public string Content { get; set; }
    }
}