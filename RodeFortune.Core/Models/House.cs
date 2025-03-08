using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class House
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("house")]
    public string HouseName { get; set; }

    [BsonElement("sign")]
    public string Sign { get; set; }

    [BsonElement("planet")]
    public string Planet { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }
}