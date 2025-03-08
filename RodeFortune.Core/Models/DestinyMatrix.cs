using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class DestinyMatrix
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("arcana")]
    public List<string> Arcana { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }
}