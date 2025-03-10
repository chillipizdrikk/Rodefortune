using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.DAL.Models
{
    public class Horoscope
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonRequired]
        [BsonElement("zodiac_sign")]
        public string ZodiacSign { get; set; }

        [BsonRequired]
        [BsonElement("motto")]
        public string Motto { get; set; }

        [BsonRequired]
        [BsonElement("content")]
        public string Content { get; set; }

        [BsonRequired]
        [BsonElement("date")]
        public DateTime Date { get; set; }
    }
}