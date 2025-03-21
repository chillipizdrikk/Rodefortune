using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.DAL.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("author")]
        public ObjectId Author { get; set; }

        [BsonRequired]
        [BsonElement("content")]
        public string Content { get; set; }

        [BsonRequired]
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("updated_at")]
        public DateTime? UpdatedAt { get; set; } = null;

        [BsonElement("image_url")]
        public byte[] ImageUrl { get; set; } = null;

        [BsonElement("referenced_reading")]
        public ObjectId? ReferencedReading { get; set; } = null;

        [BsonElement("referenced_horoscope")]
        public ObjectId? ReferencedHoroscope { get; set; } = null;

        [BsonElement("referenced_natal_chart")]
        public ObjectId? ReferencedNatalChart { get; set; } = null;

        [BsonElement("referenced_destiny_matrix")]
        public ObjectId? ReferencedDestinyMatrix { get; set; } = null;

        [BsonElement("comments")]
        public List<ObjectId> Comments { get; set; } = new();
    }
}
