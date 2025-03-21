using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.DAL.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("username")]
        public string Username { get; set; }

        [BsonRequired]
        [BsonElement("email")]
        public string Email { get; set; }

        [BsonRequired]
        [BsonElement("password_hash")]
        public string PasswordHash { get; set; }

        [BsonRequired]
        [BsonElement("birth_date")]
        public DateTime BirthDate { get; set; }

        [BsonRequired]
        [BsonElement("role")]
        public string Role { get; set; }

        [BsonRequired]
        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("zodiac_sign")]
        public string ZodiacSign { get; set; }

        [BsonElement("avatar")]
        public byte[] Avatar { get; set; } = null;

        [BsonElement("saved_readings")]
        public List<ObjectId> SavedReadings { get; set; } = new();

        [BsonElement("saved_horoscopes")]
        public List<ObjectId> SavedHoroscopes { get; set; } = new();

        [BsonElement("natal_chart")]
        public ObjectId? NatalChart { get; set; } = null;

        [BsonElement("destiny_matrix")]
        public ObjectId? DestinyMatrix { get; set; } = null;
    }
}
