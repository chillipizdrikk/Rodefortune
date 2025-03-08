using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.Core.Models
{
    public class Post
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("author_id")]
        public string AuthorId { get; set; } = string.Empty;

        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [BsonElement("image_url")]
        public List<byte[]> ImageUrls { get; set; } = new();

        [BsonElement("referenced_reading")]
        public List<string> ReferencedReading { get; set; } = new();

        [BsonElement("referenced_horoscope")]
        public string? ReferencedHoroscope { get; set; }

        [BsonElement("referenced_natal_chart")]
        public string? ReferencedNatalChart { get; set; }

        [BsonElement("referenced_destiny_matrix")]
        public string? ReferencedDestinyMatrix { get; set; }

        [BsonElement("comments")]
        public List<string> Comments { get; set; } = new();
    }
}
