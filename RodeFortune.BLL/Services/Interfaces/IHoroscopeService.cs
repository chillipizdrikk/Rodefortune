using RodeFortune.BLL.Dto;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RodeFortune.BLL.Services.Interfaces
{
    public interface IHoroscopeService
    {
        Task<List<HoroscopeResponseDto>> GetAllHoroscopesAsync();
        Task<HoroscopeResponseDto> GetHoroscopeByIdAsync(string id);
        Task<List<HoroscopeResponseDto>> GetHoroscopesByZodiacSignAsync(string zodiacSign);
        Task<List<HoroscopeResponseDto>> GetHoroscopesByDateAsync(DateTime date);
        Task<HoroscopeResponseDto> CreateHoroscopeAsync(HoroscopeRequestDto horoscopeDto);
        Task<HoroscopeResponseDto> UpdateHoroscopeAsync(string id, HoroscopeRequestDto horoscopeDto);
        Task DeleteHoroscopeAsync(string id);
    }
}