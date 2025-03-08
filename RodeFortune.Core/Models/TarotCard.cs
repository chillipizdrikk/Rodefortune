using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class TarotCard
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("arcana")]
    public string Arcana { get; set; }

    [BsonElement("reversal")]
    public bool Reversal { get; set; }

    [BsonElement("motto")]
    public string Motto { get; set; }

    [BsonElement("meaning")]
    public string Meaning { get; set; }

    [BsonElement("image_url")]
    public byte[] ImageUrl { get; set; }
}
