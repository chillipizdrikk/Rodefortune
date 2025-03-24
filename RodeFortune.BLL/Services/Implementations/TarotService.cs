using MongoDB.Driver;
using RodeFortune.BLL.Services.Interfaces;
using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodeFortune.BLL.Services.Implementations
{
    public class TarotService : IConstantDivinationService
    {
        private readonly IMongoCollection<TarotCard> _tarotCards;

        public TarotService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("RodeFortune");
            _tarotCards = database.GetCollection<TarotCard>("TarotCards");
        }

        public async Task<TarotCard> GetTarotCardByBirthDateAsync(DateTime birthDate)
        {
            int cardNumber = CalculateTarotNumber(birthDate);
            var filter = Builders<TarotCard>.Filter.Eq("arcana", "Major");
            var majorArcanaCards = await _tarotCards.Find(filter).ToListAsync();
            if (majorArcanaCards.Count == 0)
            {
                return await _tarotCards.Find(Builders<TarotCard>.Filter.Empty).FirstOrDefaultAsync();
            }
            int cardIndex = cardNumber % majorArcanaCards.Count;
            var selectedCard = majorArcanaCards[cardIndex];

            selectedCard.Reversal = ShouldCardBeReversed(birthDate);

            return selectedCard;
        }

        private int CalculateTarotNumber(DateTime birthDate)
        {

            int day = birthDate.Day;
            int month = birthDate.Month;
            int year = birthDate.Year;

            int sum = SumDigits(day) + SumDigits(month) + SumDigits(year);

            while (sum > 21)
            {
                sum = SumDigits(sum);
            }

            return sum;
        }

        private int SumDigits(int number)
        {
            int sum = 0;
            while (number > 0)
            {
                sum += number % 10;
                number /= 10;
            }
            return sum == 0 ? number : sum;
        }

        private bool ShouldCardBeReversed(DateTime birthDate)
        {
            return birthDate.Day % 2 == 1;
        }

    }
}
