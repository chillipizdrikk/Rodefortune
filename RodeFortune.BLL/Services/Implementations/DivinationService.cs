using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using RodeFortune.BLL.Models;
using RodeFortune.BLL.Services.Interfaces;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using System.Runtime.CompilerServices;

namespace RodeFortune.BLL.Services.Implementations
{
    public class DivinationService : IDivinationService
    {
        private readonly ITarotCardRepository _tarotCardRepository;
        private readonly ILogger<DivinationService> _logger;
        private readonly Random _random;
        private Dictionary<string, (string CardId, bool IsReversed)> _dailyCards;
        private const int NUMBER_OF_CARDS = 3;
        public DivinationService(ITarotCardRepository tarotCardRepository, ILogger<DivinationService> logger)
        {
            _tarotCardRepository = tarotCardRepository;
            _logger = logger;
            _random = new Random();
            _dailyCards = new Dictionary<string, (string, bool)>();
        }

        public async Task<(TarotCard Card, bool IsReversed)> GetYesNoReadingAsync()
        {
            _logger.LogDebug("Запит на Yes/No гадання");
            var card = await GetRandomCardAsync();
            bool isReversed = _random.Next(2) == 1;
            return (card, isReversed);
        }

        public async Task<List<(TarotCard Card, bool IsReversed, string Position)>> GetPastPresentFutureReadingAsync()
        {
            var allCards = await _tarotCardRepository.GetAllAsync();
            var result = new List<(TarotCard, bool, string)>();
            var positions = new[] { "Минуле", "Теперішнє", "Майбутнє" };

            var availableCards = allCards.ToList();

            for (int i = 0; i < NUMBER_OF_CARDS; i++)
            {
                int index = _random.Next(availableCards.Count);
                bool isReversed = _random.Next(2) == 1;
                result.Add((availableCards[index], isReversed, positions[i]));
                availableCards.RemoveAt(index);
            }

            return result;
        }

        public async Task<List<(TarotCard Card, bool IsReversed, string Position)>> GetCaseActionResultReadingAsync()
        {
            var allCards = await _tarotCardRepository.GetAllAsync();
            var result = new List<(TarotCard, bool, string)>();
            var positions = new[] { "Ситуація", "Дія", "Результат" };

            var availableCards = allCards.ToList();
            var cardsCount = 3;

            for (int i = 0; i < cardsCount; i++)
            {
                int index = _random.Next(availableCards.Count);
                bool isReversed = _random.Next(2) == 1;
                result.Add((availableCards[index], isReversed, positions[i]));
                availableCards.RemoveAt(index);
            }

            return result;
        }

        public async Task<List<(TarotCard Card, bool IsReversed, string Position)>> GetDreamReviewReadingAsync()
        {
            var allCards = await _tarotCardRepository.GetAllAsync();
            var result = new List<(TarotCard, bool, string)>();
            var positions = new[] { "Символ", "Значення", "Порада" };

            var availableCards = allCards.ToList();

            for (int i = 0; i < NUMBER_OF_CARDS; i++)
            {
                int index = _random.Next(availableCards.Count);
                bool isReversed = _random.Next(2) == 1;
                result.Add((availableCards[index], isReversed, positions[i]));
                availableCards.RemoveAt(index);
            }

            return result;
        }

        public async Task<List<(TarotCard Card, bool IsReversed, string Position)>> GetProblemSolutionReadingAsync()
        {
            var allCards = await _tarotCardRepository.GetAllAsync();
            var result = new List<(TarotCard, bool, string)>();
            var positions = new[] { "Проблема", "Вирішення" };

            var availableCards = allCards.ToList();
            var cardsCount = 2;

            for (int i = 0; i < cardsCount; i++)
            {
                int index = _random.Next(availableCards.Count);
                bool isReversed = _random.Next(2) == 1;
                result.Add((availableCards[index], isReversed, positions[i]));
                availableCards.RemoveAt(index);
            }

            return result;
        }

        public async Task<Result<(TarotCard Card, bool IsReversed, bool IsNew)>> GetCardOfTheDayAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("User ID cannot be null or empty");
                return new Result<(TarotCard, bool, bool)>(false,
                    "User ID cannot be null or empty"
                );
            }

            try
            {
                _logger.LogDebug($"Attempting to retrieve card for user {userId}");
                string key = $"{userId}_{DateTime.Today:yyyyMMdd}";

                if (_dailyCards.TryGetValue(key, out var cardInfo))
                {
                    _logger.LogDebug($"Found cached card info for user {userId}");
                    var card = await _tarotCardRepository.GetCardByIdAsync(cardInfo.CardId);

                    if (card == null)
                    {
                        _logger.LogWarning($"Failed to retrieve card with ID {cardInfo.CardId} from repository");
                        return new Result<(TarotCard, bool, bool)>(false, "Failed to retrieve card from repository");
                    }

                    _logger.LogInformation($"Successfully retrieved daily card for user {userId}");
                    return new Result<(TarotCard, bool, bool)>(
                        true,
                        "Successfully retrieved daily card",
                        (card, cardInfo.IsReversed, false)
                    );
                }

                _logger.LogDebug($"No cached card found for user {userId}, generating new card");
                var newCard = await GetRandomCardAsync();

                if (newCard == null)
                {
                    _logger.LogWarning("Failed to generate random card");
                    return new Result<(TarotCard, bool, bool)>(false, "Failed to generate random card");
                }

                bool isReversed = _random.Next(2) == 1;
                _dailyCards[key] = (newCard.Id.ToString(), isReversed);

                _logger.LogInformation($"Successfully generated new daily card for user {userId}");
                return new Result<(TarotCard, bool, bool)>(
                    true,
                    "Successfully generated new daily card",
                    (newCard, isReversed, true)
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving card of the day for user {userId}");
                return new Result<(TarotCard, bool, bool)>(
                    false,
                    $"Error retrieving card of the day: {ex.Message}"
                );
            }
        }

        private async Task<TarotCard> GetRandomCardAsync()
        {
            var cards = await _tarotCardRepository.GetAllAsync();
            return cards.OrderBy(_ => _random.Next()).First();
        }

        public async Task<List<TarotCard>> GetCardsAsync(string? searchTerm = null, string? arcana = null)
        {
            var allCards = await _tarotCardRepository.GetAllAsync();

            var filteredCards = allCards.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                filteredCards = filteredCards.Where(c =>
                    c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (c.Meaning != null && c.Meaning.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (c.Motto != null && c.Motto.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrWhiteSpace(arcana))
            {
                filteredCards = filteredCards.Where(c =>
                    c.Arcana != null && c.Arcana.Equals(arcana, StringComparison.OrdinalIgnoreCase));
            }

            return filteredCards.ToList();
        }


    }
}