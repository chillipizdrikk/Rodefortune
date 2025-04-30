using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.DAL.Models
{
    public class NatalChart
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("user_id")]
        public ObjectId UserId { get; set; }

        [BsonRequired]
        [BsonElement("houses")]
        public List<ObjectId> Houses { get; set; } = new();

        [BsonRequired]
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("image_url")]
        public byte[] ImageUrl { get; set; } = null;
    }
}
