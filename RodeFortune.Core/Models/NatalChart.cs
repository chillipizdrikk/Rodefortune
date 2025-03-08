using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class NatalChart
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("houses")]
    public List<string> Houses { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("image_url")]
    public byte[] ImageUrl { get; set; }
}