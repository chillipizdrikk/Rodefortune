using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class DestinyMatrixRequestDto
    {
        public ObjectId UserId { get; set; }
        public string Content { get; set; }
    }

    public class DestinyMatrixResponseDto
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }
    }
}
