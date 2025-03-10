using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class TarotCardRequestDto
    {
        public string Name { get; set; }
        public string Arcana { get; set; }
        public bool Reversal { get; set; }
        public string Motto { get; set; }
        public string Meaning { get; set; }
        public byte[] ImageUrl { get; set; }
    }

    public class TarotCardResponseDto
    {
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string Arcana { get; set; }
        public bool Reversal { get; set; }
        public string Motto { get; set; }
        public string Meaning { get; set; }
        public byte[] ImageUrl { get; set; }
    }
}
