using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.DAL.Models
{
    public class Reading
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("author_id")]
        public ObjectId AuthorId { get; set; }

        [BsonRequired]
        [BsonElement("type")]
        public string Type { get; set; }

        [BsonRequired]
        [BsonElement("cards")]
        public List<ObjectId> Cards { get; set; } = new();

        [BsonRequired]
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
