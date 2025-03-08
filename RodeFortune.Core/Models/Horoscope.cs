using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

public class Horoscope
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("zodiac_sign")]
    public string ZodiacSign { get; set; }

    [BsonElement("motto")]
    public string Motto { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }

    [BsonElement("date")]
    public DateTime Date { get; set; }
}