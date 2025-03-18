using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using RodeFortune.BLL.Services.Interfaces;

namespace RodeFortune.BLL.Services.Implementations
{
    public class DivinationService : IDivinationService
    {
        private readonly ITarotCardRepository _tarotCardRepository;
        private readonly ILogger<DivinationService> _logger;
        private readonly Random _random;
        private Dictionary<string, (string CardId, bool IsReversed)> _dailyCards;

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

            for (int i = 0; i < 3; i++)
            {
                int index = _random.Next(availableCards.Count);
                bool isReversed = _random.Next(2) == 1;
                result.Add((availableCards[index], isReversed, positions[i]));
                availableCards.RemoveAt(index);
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

        private async Task<TarotCard> GetRandomCardAsync()
        {
            var cards = await _tarotCardRepository.GetAllAsync();
            return cards.OrderBy(_ => _random.Next()).First();
        }

        public async Task<List<TarotCard>> GetCardsAsync(string searchTerm = null, string arcana = null)
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