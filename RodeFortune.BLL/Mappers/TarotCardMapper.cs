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
                Motto = tarotCard.Motto,
                Meaning = tarotCard.Meaning,
                ReversalMeaning = tarotCard.ReversalMeaning,
                RomanceMeaning = tarotCard.RomanceMeaning,
                FinanceMeaning = tarotCard.FinanceMeaning,
                HealthMeaning = tarotCard.HealthMeaning,
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
                Motto = tarotCardRequestDto.Motto,
                Meaning = tarotCardRequestDto.Meaning,
                ReversalMeaning = tarotCardRequestDto.ReversalMeaning,
                RomanceMeaning = tarotCardRequestDto.RomanceMeaning,
                FinanceMeaning = tarotCardRequestDto.FinanceMeaning,
                HealthMeaning = tarotCardRequestDto.HealthMeaning,
                ImageUrl = tarotCardRequestDto.ImageUrl
            };
        }
    }
}
