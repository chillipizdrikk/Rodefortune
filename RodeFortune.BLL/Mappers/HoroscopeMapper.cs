using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;
using System;

namespace RodeFortune.BLL.Mappers
{
    public static class HoroscopeMapper
    {
        public static HoroscopeResponseDto ToDto(this Horoscope horoscope)
        {
            if (horoscope == null) return null;

            return new HoroscopeResponseDto
            {
                Id = horoscope.Id,
                ZodiacSign = horoscope.ZodiacSign,
                Motto = horoscope.Motto,
                Content = horoscope.Content,
                Date = horoscope.Date
            };
        }

        public static Horoscope ToEntity(this HoroscopeRequestDto horoscopeDto)
        {
            if (horoscopeDto == null) return null;

            return new Horoscope
            {
                ZodiacSign = horoscopeDto.ZodiacSign,
                Motto = horoscopeDto.Motto,
                Content = horoscopeDto.Content,
                Date = horoscopeDto.Date
            };
        }
    }
}
