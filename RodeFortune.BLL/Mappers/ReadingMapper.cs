using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;

namespace RodeFortune.BLL.Mappers
{
    public static class ReadingMapper
    {
        public static ReadingResponseDto ToDto(this Reading reading)
        {
            if (reading == null) return null;

            return new ReadingResponseDto
            {
                Id = reading.Id,
                AuthorId = reading.AuthorId,
                Type = reading.Type,
                Cards = reading.Cards,
                CreatedAt = reading.CreatedAt
            };
        }

        public static Reading ToEntity(this ReadingRequestDto readingRequestDto)
        {
            if (readingRequestDto == null) return null;

            return new Reading
            {
                AuthorId = readingRequestDto.AuthorId,
                Type = readingRequestDto.Type,
                Cards = readingRequestDto.Cards,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
