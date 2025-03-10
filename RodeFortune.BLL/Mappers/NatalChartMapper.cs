using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;

namespace RodeFortune.BLL.Mappers
{
    public static class NatalChartMapper
    {
        public static NatalChartResponseDto ToDto(this NatalChart natalChart)
        {
            if (natalChart == null) return null;

            return new NatalChartResponseDto
            {
                Id = natalChart.Id,
                UserId = natalChart.UserId,
                Houses = natalChart.Houses,
                CreatedAt = natalChart.CreatedAt,
                ImageUrl = natalChart.ImageUrl
            };
        }

        public static NatalChart ToEntity(this NatalChartRequestDto natalChartRequestDto)
        {
            if (natalChartRequestDto == null) return null;

            return new NatalChart
            {
                UserId = natalChartRequestDto.UserId,
                Houses = natalChartRequestDto.Houses,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = natalChartRequestDto.ImageUrl
            };
        }
    }
}
