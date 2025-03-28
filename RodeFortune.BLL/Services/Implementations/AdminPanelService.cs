using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using RodeFortune.BLL.Models;
using RodeFortune.BLL.Services.Interfaces;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;

namespace RodeFortune.BLL.Services.Implementations
{
    public class AdminPanelService : IAdminPanelService
    {
        private readonly ITarotCardRepository _tarotRepository;
        private readonly IHoroscopeRepository _horoscopeRepository;
        private readonly ILogger<AdminPanelService> _logger;

        public AdminPanelService(ITarotCardRepository tarotRepository, IHoroscopeRepository horoscopeRepository, ILogger<AdminPanelService> logger)
        {
            _tarotRepository = tarotRepository;
            _horoscopeRepository = horoscopeRepository;
            _logger = logger;
        }

        public async Task<Result<Horoscope>> CreateHoroscopeAsync(string zodiacSign, string motto, string content, DateTime date)
        {
            if (string.IsNullOrWhiteSpace(zodiacSign))
            {
                _logger.LogInformation($"Tarot card name was empty or null");
                return new Result<Horoscope>(false, "Tarot card name  was empty or null", null);
            }
            if (string.IsNullOrWhiteSpace(motto))
            {
                _logger.LogInformation($"Tarot card name was empty or null");
                return new Result<Horoscope>(false, "Tarot card name  was empty or null", null);
            }
            if (string.IsNullOrWhiteSpace(content))
            {
                _logger.LogInformation($"Tarot card name was empty or null");
                return new Result<Horoscope>(false, "Tarot card name  was empty or null", null);
            }

            try
            {
                var horoscope = new Horoscope
                {
                    ZodiacSign = zodiacSign,
                    Motto = motto,
                    Content = content,
                    Date = DateTime.SpecifyKind(date.Date, DateTimeKind.Utc)
                };

                await _horoscopeRepository.CreateAsync(horoscope);

                _logger.LogInformation($"Horoscope created successfully for {zodiacSign} on {date}");
                return new Result<Horoscope>(true, "Horoscope created successfully", horoscope);
            }
            catch (Exception ex)
            {
                return new Result<Horoscope>(false, $"Error while creating horoscope: {ex.Message}", null);
            }
        }

        public async Task<Result<Horoscope>> UpdateHoroscopeAsync(ObjectId id, string? zodiacSign = null, string? motto = null, string? content = null, DateTime? date = null)
        {
            try
            {
                var existingHoroscope = await _horoscopeRepository.GetByIdAsync(id);

                if (existingHoroscope == null)
                {
                    _logger.LogWarning($"Horoscope with ID {id} not found");
                    return new Result<Horoscope>(false, "Horoscope not found", null);
                }

                if (!string.IsNullOrWhiteSpace(zodiacSign))
                    existingHoroscope.ZodiacSign = zodiacSign.Trim();

                if (!string.IsNullOrWhiteSpace(motto))
                    existingHoroscope.Motto = motto.Trim();

                if (!string.IsNullOrWhiteSpace(content))
                    existingHoroscope.Content = content.Trim();

                if (date.HasValue)
                {
                    existingHoroscope.Date = DateTime.SpecifyKind(date.Value, DateTimeKind.Utc);
                }

                await _horoscopeRepository.UpdateAsync(id, existingHoroscope);

                _logger.LogInformation($"Horoscope {id} updated successfully");
                return new Result<Horoscope>(true, "Horoscope updated successfully", existingHoroscope);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while updating horoscope {id}");
                return new Result<Horoscope>(false, $"Error while updating horoscope: {ex.Message}", null);
            }
        }

        public async Task<Result<bool>> DeleteHoroscopeAsync(ObjectId id)
        {
            try
            {
                var existingHoroscope = await _horoscopeRepository.GetByIdAsync(id);

                if (existingHoroscope == null)
                {
                    _logger.LogWarning($"Horoscope with ID {id} not found for deletion");
                    return new Result<bool>(false, "Horoscope not found", false);
                }

                await _horoscopeRepository.DeleteAsync(id);

                _logger.LogInformation($"Horoscope {id} deleted successfully");
                return new Result<bool>(true, "Horoscope deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while deleting horoscope {id}");
                return new Result<bool>(false, $"Error while deleting horoscope: {ex.Message}", false);
            }
        }

        public async Task<Result<TarotCard>> CreateTarotCardAsync(string name, string arcana, string motto,
    string meaning, bool reversal, byte[]? imageUrl = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                _logger.LogInformation($"Tarot card name was empty or null");
                return new Result<TarotCard>(false, "Tarot card name  was empty or null", null);
            }

            if (string.IsNullOrWhiteSpace(arcana))
            {

                _logger.LogInformation($"Arcana name was empty or null");
                return new Result<TarotCard>(false, "Arcana card name was empty or null", null);
            }

            if (string.IsNullOrWhiteSpace(motto))
            {
                _logger.LogInformation($"Motto was empty or null");
                return new Result<TarotCard>(false, "Motto was empty or null", null);
            }

            if (string.IsNullOrWhiteSpace(meaning))
            {
                _logger.LogInformation($"Meaning was empty or null");
                return new Result<TarotCard>(false, "Meaning was empty or null", null);
            }

            try
            {
                var tarotCard = new TarotCard
                {
                    Name = name,
                    Arcana = arcana,
                    Reversal = reversal,
                    Motto = motto,
                    Meaning = meaning,
                    ImageUrl = imageUrl
                };

                await _tarotRepository.CreateAsync(tarotCard);

                _logger.LogInformation("Created tarot card with name: {Name} and arcana: {Arcana}", name, arcana);

                return new Result<TarotCard>(true, "Tarot card created successfully", tarotCard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating tarot card '{name}'");
                return new Result<TarotCard>(false, $"Error while creating tarot card: {ex.Message}", null);
            }
        }

        //Because we have a different name for every card, we can search our cards by names - see more in AdminController
        public async Task<Result<bool>> DeleteTarotCardByNameAsync(string tarotCardName)
        {
            if (string.IsNullOrWhiteSpace(tarotCardName))
            {
                _logger.LogInformation("Tarot card name was empty or null");
                return new Result<bool>(false, "Tarot card name was empty or null", false);
            }

            try
            {
                var tarotCard = await _tarotRepository.GetCardByNameAsync(tarotCardName);

                if (tarotCard == null)
                {
                    _logger.LogInformation("Tarot card with name {TarotCardName} not found", tarotCardName);
                    return new Result<bool>(false, "Tarot card not found", false);
                }

                await _tarotRepository.DeleteByNameAsync(tarotCard.Name);

                _logger.LogInformation("Successfully deleted tarot card with name {TarotCardName}", tarotCardName);

                return new Result<bool>(true, "Tarot card deleted successfully", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tarot card with name {TarotCardName}", tarotCardName);
                return new Result<bool>(false, $"Error while deleting tarot card: {ex.Message}", false);
            }
        }
    }
}
