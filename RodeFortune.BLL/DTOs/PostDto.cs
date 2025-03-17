using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class PostRequestDto
    {
        public ObjectId Author { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public byte[] ImageUrl { get; set; }

        public ObjectId? ReferencedReading { get; set; }
        public ObjectId? ReferencedHoroscope { get; set; }
        public ObjectId? ReferencedNatalChart { get; set; }
        public ObjectId? ReferencedDestinyMatrix { get; set; }
    }

    public class PostResponseDto
    {
        public ObjectId Id { get; set; }
        public ObjectId Author { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Name { get; set; }
        public byte[] ImageUrl { get; set; }
        public ObjectId? ReferencedReading { get; set; }
        public ObjectId? ReferencedHoroscope { get; set; }
        public ObjectId? ReferencedNatalChart { get; set; }
        public ObjectId? ReferencedDestinyMatrix { get; set; }
        public List<ObjectId> Comments { get; set; }
    }
}
