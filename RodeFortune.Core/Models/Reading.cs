using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Reading
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("author_id")]
    public string AuthorId { get; set; }

    [BsonElement("type")]
    public string Type { get; set; }

    [BsonElement("cards")]
    public List<string> Cards { get; set; } = new();

    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }
}
