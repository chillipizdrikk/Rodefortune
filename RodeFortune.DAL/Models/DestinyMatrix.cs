using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.DAL.Models
{
    public class DestinyMatrix
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("user_id")]
        public ObjectId UserId { get; set; }

        [BsonRequired]
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("content")]
        public string Content { get; set; } = null;
    }
}
