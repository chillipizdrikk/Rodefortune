using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.DAL.Models
{
    public class Comment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("post_id")]
        public ObjectId PostId { get; set; }

        [BsonRequired]
        [BsonElement("author_id")]
        public ObjectId AuthorId { get; set; }

        [BsonRequired]
        [BsonElement("content")]
        public string Content { get; set; }

        [BsonRequired]
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


    }
}
