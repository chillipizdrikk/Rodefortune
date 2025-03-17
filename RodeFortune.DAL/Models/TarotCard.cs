using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.DAL.Models
{
    public class TarotCard
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonRequired]
        [BsonElement("arcana")]
        public string Arcana { get; set; }

        [BsonRequired]
        [BsonElement("reversal")]
        public bool Reversal { get; set; }

        [BsonElement("motto")]
        public string Motto { get; set; }

        [BsonElement("meaning")]
        public string Meaning { get; set; }

        [BsonElement("image_url")]
        public byte[] ImageUrl { get; set; } = null;
    }
}
