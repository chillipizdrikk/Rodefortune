using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RodeFortune.DAL.Models
{
    public class House
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("house")]
        public string HouseNumber { get; set; }

        [BsonRequired]
        [BsonElement("sign")]
        public string Sign { get; set; }

        [BsonRequired]
        [BsonElement("planet")]
        public string Planet { get; set; }

        [BsonElement("content")]
        public string Content { get; set; }
    }
}
