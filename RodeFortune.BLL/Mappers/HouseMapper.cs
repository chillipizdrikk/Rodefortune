using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;

namespace RodeFortune.BLL.Mappers
{
    public static class HouseMapper
    {
        public static HouseResponseDto ToDto(this House house)
        {
            if (house == null) return null;

            return new HouseResponseDto
            {
                Id = house.Id,
                HouseNumber = house.HouseNumber,
                Sign = house.Sign,
                Planet = house.Planet,
                Content = house.Content
            };
        }

        public static House ToEntity(this HouseRequestDto houseRequestDto)
        {
            if (houseRequestDto == null) return null;

            return new House
            {
                HouseNumber = houseRequestDto.HouseNumber,
                Sign = houseRequestDto.Sign,
                Planet = houseRequestDto.Planet,
                Content = houseRequestDto.Content
            };
        }
    }
}
