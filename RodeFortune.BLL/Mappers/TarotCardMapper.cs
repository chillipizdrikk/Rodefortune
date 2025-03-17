using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;

namespace RodeFortune.BLL.Mappers
{
    public static class TarotCardMapper
    {
        public static TarotCardResponseDto ToDto(this TarotCard tarotCard)
        {
            if (tarotCard == null) return null;

            return new TarotCardResponseDto
            {
                Id = tarotCard.Id,
                Name = tarotCard.Name,
                Arcana = tarotCard.Arcana,
                Reversal = tarotCard.Reversal,
                Motto = tarotCard.Motto,
                Meaning = tarotCard.Meaning,
                ImageUrl = tarotCard.ImageUrl
            };
        }

        public static TarotCard ToEntity(this TarotCardRequestDto tarotCardRequestDto)
        {
            if (tarotCardRequestDto == null) return null;

            return new TarotCard
            {
                Name = tarotCardRequestDto.Name,
                Arcana = tarotCardRequestDto.Arcana,
                Reversal = tarotCardRequestDto.Reversal,
                Motto = tarotCardRequestDto.Motto,
                Meaning = tarotCardRequestDto.Meaning,
                ImageUrl = tarotCardRequestDto.ImageUrl
            };
        }
    }
}
