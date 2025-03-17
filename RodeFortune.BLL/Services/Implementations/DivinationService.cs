using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace RodeFortune.BLL.Services.Implementations
{
    public class DivinationService
    {
        private readonly IMongoCollection<TarotCard> _tarotCards;
        private readonly Random _random;
        private Dictionary<string, (string CardId, bool IsReversed)> _dailyCards;
        private readonly ITarotCardRepository _tarotCardRepository;
        private readonly ILogger<DivinationService> _logger;

        public DivinationService(IMongoDatabase database, ILogger<DivinationService> logger)
        {
            _tarotCards = database.GetCollection<TarotCard>("TarotCards");
            _random = new Random();
            _dailyCards = new Dictionary<string, (string, bool)>();
            _logger = logger;

            _logger.LogInformation("DivinationService ініціалізовано");
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
            var allCards = await _tarotCards.Find(_ => true).ToListAsync();
            var result = new List<(TarotCard, bool, string)>();
            var positions = new[] { "Минуле", "Теперішнє", "Майбутнє" };

            for (int i = 0; i < 3; i++)
            {
                int index = _random.Next(allCards.Count);
                bool isReversed = _random.Next(2) == 1;

                result.Add((allCards[index], isReversed, positions[i]));
                allCards.RemoveAt(index);
            }

            return result;
        }

        public async Task<(TarotCard Card, bool IsReversed, bool IsNew)> GetCardOfTheDayAsync(string userId)
        {
            string key = $"{userId}_{DateTime.Today:yyyyMMdd}";

            if (_dailyCards.TryGetValue(key, out var cardInfo))
                return (await _tarotCardRepository.GetCardByIdAsync(cardInfo.CardId), cardInfo.IsReversed, false);

            var newCard = await GetRandomCardAsync();
            bool isReversed = _random.Next(2) == 1;

            _dailyCards[key] = (newCard.Id.ToString(), isReversed);
            return (newCard, isReversed, true);
        }

        private async Task<TarotCard> GetRandomCardAsync() =>
        (await _tarotCards.Find(_ => true).ToListAsync()).OrderBy(_ => _random.Next()).First();
    }
}