using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class CommentRequestDto
    {
        public ObjectId PostId { get; set; }

        public ObjectId AuthorId { get; set; }

        public string Content { get; set; }
    }

    public class CommentResponseDto
    {
        public ObjectId Id { get; set; }
        public ObjectId PostId { get; set; }
        public ObjectId AuthorId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
