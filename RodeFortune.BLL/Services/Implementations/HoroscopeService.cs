using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.BLL.Mappers;
using RodeFortune.BLL.Services.Interfaces;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RodeFortune.BLL.Services.Implementations
{
    public class HoroscopeService : IHoroscopeService
    {
        private readonly HoroscopeRepository _horoscopeRepository;
        private readonly ILogger<HoroscopeService> _logger;

        public HoroscopeService(HoroscopeRepository horoscopeRepository, ILogger<HoroscopeService> logger)
        {
            _horoscopeRepository = horoscopeRepository;
            _logger = logger;
        }

        public async Task<List<HoroscopeResponseDto>> GetAllHoroscopesAsync()
        {
            try
            {
                _logger.LogInformation("Отримання всіх гороскопів");
                var horoscopes = await _horoscopeRepository.GetAllAsync();
                return horoscopes.Select(h => h.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні всіх гороскопів");
                throw;
            }
        }

        public async Task<HoroscopeResponseDto> GetHoroscopeByIdAsync(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                {
                    _logger.LogWarning("Недійсний ID гороскопу: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Отримання гороскопу за ID: {Id}", id);
                var horoscope = await _horoscopeRepository.GetByIdAsync(objectId);
                return horoscope?.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гороскопу за ID: {Id}", id);
                throw;
            }
        }

        public async Task<List<HoroscopeResponseDto>> GetHoroscopesByZodiacSignAsync(string zodiacSign)
        {
            try
            {
                _logger.LogInformation("Отримання гороскопів за знаком зодіаку: {ZodiacSign}", zodiacSign);
                var horoscopes = await _horoscopeRepository.GetByZodiacSignAsync(zodiacSign);
                return horoscopes.Select(h => h.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гороскопів за знаком зодіаку: {ZodiacSign}", zodiacSign);
                throw;
            }
        }

        public async Task<List<HoroscopeResponseDto>> GetHoroscopesByDateAsync(DateTime date)
        {
            try
            {
                _logger.LogInformation("Отримання гороскопів за датою: {Date}", date.ToShortDateString());
                
                var startDate = date.Date;
                var endDate = startDate.AddDays(1).AddTicks(-1);
                
                var horoscopes = await _horoscopeRepository.GetByDateRangeAsync(startDate, endDate);
                return horoscopes.Select(h => h.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гороскопів за датою: {Date}", date.ToShortDateString());
                throw;
            }
        }

        public async Task<HoroscopeResponseDto> CreateHoroscopeAsync(HoroscopeRequestDto horoscopeDto)
        {
            try
            {
                _logger.LogInformation("Створення нового гороскопу для знаку: {ZodiacSign}", horoscopeDto.ZodiacSign);
                var horoscope = horoscopeDto.ToEntity();
                await _horoscopeRepository.CreateAsync(horoscope);
                return horoscope.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні гороскопу для знаку: {ZodiacSign}", horoscopeDto.ZodiacSign);
                throw;
            }
        }

        public async Task<HoroscopeResponseDto> UpdateHoroscopeAsync(string id, HoroscopeRequestDto horoscopeDto)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                {
                    _logger.LogWarning("Недійсний ID гороскопу для оновлення: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Оновлення гороскопу з ID: {Id}", id);
                
                var existingHoroscope = await _horoscopeRepository.GetByIdAsync(objectId);
                if (existingHoroscope == null)
                {
                    _logger.LogWarning("Гороскоп з ID {Id} не знайдено для оновлення", id);
                    return null;
                }

                var updatedHoroscope = horoscopeDto.ToEntity();
                updatedHoroscope.Id = objectId;
                
                await _horoscopeRepository.UpdateAsync(objectId, updatedHoroscope);
                return updatedHoroscope.ToDto();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні гороскопу з ID: {Id}", id);
                throw;
            }
        }

        public async Task DeleteHoroscopeAsync(string id)
        {
            try
            {
                if (!ObjectId.TryParse(id, out var objectId))
                {
                    _logger.LogWarning("Недійсний ID гороскопу для видалення: {Id}", id);
                    return;
                }

                _logger.LogInformation("Видалення гороскопу з ID: {Id}", id);
                await _horoscopeRepository.DeleteAsync(objectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні гороскопу з ID: {Id}", id);
                throw;
            }
        }
    }
}