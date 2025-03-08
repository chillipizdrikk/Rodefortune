using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.Core.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        [BsonElement("username")]
        public string Username { get; set; } = string.Empty;

        [BsonElement("email")]
        public string Email { get; set; } = string.Empty;

        [BsonElement("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [BsonElement("birth_date")]
        public DateTime BirthDate { get; set; }

        [BsonElement("zodiac_sign")]
        public string ZodiacSign { get; set; } = string.Empty;

        [BsonElement("role")]
        public string Role { get; set; } = string.Empty;

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("avatar")]
        public byte[]? Avatar { get; set; }

        [BsonElement("saved_readings")]
        public List<string> SavedReadings { get; set; } = new();

        [BsonElement("saved_horoscopes")]
        public List<string> SavedHoroscopes { get; set; } = new();

        [BsonElement("natal_chart")]
        public string? NatalChartId { get; set; }

        [BsonElement("destiny_matrix")]
        public string? DestinyMatrixId { get; set; }
    }
}
