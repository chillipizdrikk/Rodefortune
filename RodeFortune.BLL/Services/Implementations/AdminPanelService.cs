using Microsoft.Extensions.Logging;
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

















        //public async Task<Result<TarotCard>> EditTarotCardAsync(string existingName, string newName, string arcana, string motto, string meaning, bool reversal, byte[]? imageUrl = null)
        //{
        //    if (string.IsNullOrWhiteSpace(existingName))
        //    {
        //        _logger.LogInformation("Existing tarot card name was empty or null");
        //        return new Result<TarotCard>(false, "Existing tarot card name was empty or null", null);
        //    }

        //    try
        //    {
        //        var tarotCard = await _tarotRepository.GetCardByNameAsync(existingName);

        //        if (tarotCard == null)
        //        {
        //            _logger.LogInformation("Tarot card with name {ExistingName} not found", existingName);
        //            return new Result<TarotCard>(false, "Tarot card not found", null);
        //        }

        //        tarotCard.Name = !string.IsNullOrWhiteSpace(newName) ? newName : tarotCard.Name;
        //        tarotCard.Arcana = !string.IsNullOrWhiteSpace(arcana) ? arcana : tarotCard.Arcana;
        //        tarotCard.Motto = !string.IsNullOrWhiteSpace(motto) ? motto : tarotCard.Motto;
        //        tarotCard.Meaning = !string.IsNullOrWhiteSpace(meaning) ? meaning : tarotCard.Meaning;
        //        tarotCard.Reversal = reversal;  
        //        tarotCard.ImageUrl = imageUrl ?? tarotCard.ImageUrl; 

        //        await _tarotRepository.UpdateAsync(tarotCard.Id, tarotCard);

        //        _logger.LogInformation("Updated tarot card: {ExistingName} -> {NewName}", existingName, newName);

        //        return new Result<TarotCard>(true, "Tarot card updated successfully", tarotCard);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error updating tarot card with name {ExistingName}", existingName);
        //        return new Result<TarotCard>(false, $"Error while updating tarot card: {ex.Message}", null);
        //    }
        //}
    }
}
