using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;


namespace RodeFortune.UnitTests.Services
{
    [TestFixture]
    public class DivinationServiceTests
    {
        private DivinationService _divinationService;
        private Mock<ITarotCardRepository> _mockTarotCardRepository;
        private Mock<ILogger<DivinationService>> _mockLogger;
        private List<TarotCard> _testCards;
        private Dictionary<string, (string CardId, bool IsReversed)> _dailyCards;


        [SetUp]
        public void Setup()
        {
            _mockTarotCardRepository = new Mock<ITarotCardRepository>();
            _mockLogger = new Mock<ILogger<DivinationService>>();

            _testCards = new List<TarotCard>
            {
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Колесо Фортуни", Arcana = "Major" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Маг", Arcana = "Major" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Смерть", Arcana = "Major" }

            };

            _mockTarotCardRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_testCards);
            _divinationService = new DivinationService(_mockTarotCardRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetYesNoReadingAsync_ShouldReturnValidCard()
        {
            var result = await _divinationService.GetYesNoReadingAsync();
            Assert.That(result.Card, Is.Not.Null, "Повинна повертатись карта");
            Assert.That(result.Card.Name, Is.AnyOf("Колесо Фортуни", "Маг", "Смерть"),
                "Назва карти повинна бути однією з тестових карт");
        }

        [Test]
        public async Task GetYesNoReadingAsync_ShouldReturnCardWithCorrectProperties()
        {
            var result = await _divinationService.GetYesNoReadingAsync();
            Assert.That(result.Card, Is.Not.Null, "Повинна повертатись карта");
            Assert.That(result.Card.Arcana, Is.EqualTo("Major"),
                "Карта повинна належати до Старших Арканів");
            Assert.That(result.IsReversed, Is.AnyOf(true, false),
                "Карта має бути перевернутою або ні");
        }

        [Test]
        public async Task GetPastPresentFutureReadingAsync_ShouldReturnThreeCards()
        {
            var result = await _divinationService.GetPastPresentFutureReadingAsync();
            Assert.That(result, Is.Not.Null, "Результат не може бути null");
            Assert.That(result.Count, Is.EqualTo(3), "Повинно бути повернуто 3 карти");
        }

        [Test]
        public async Task GetPastPresentFutureReadingAsync_ShouldReturnUniqueCards()
        {
            var result = await _divinationService.GetPastPresentFutureReadingAsync();
            var uniqueCardNames = result.Select(card => card.Card.Name).Distinct().Count();
            Assert.That(uniqueCardNames, Is.EqualTo(3), "Всі карти мають бути унікальними");
        }

        [Test]
        public async Task GetCardOfTheDayAsync_ShouldReturnSameCardForSameUserOnSameDay()
        {
            string userId = "user1";
            var firstCardId = "";
            bool firstCardIsReversed = false;

            var firstResult = await _divinationService.GetCardOfTheDayAsync(userId);
            firstCardId = firstResult.Card.Name;
            firstCardIsReversed = firstResult.IsReversed;

            _mockTarotCardRepository.Setup(repo => repo.GetCardByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(firstResult.Card);

            var secondResult = await _divinationService.GetCardOfTheDayAsync(userId);

            Assert.That(secondResult.IsNew, Is.False, "Другий запит має повернути існуючу карту");
            Assert.That(secondResult.Card.Name, Is.EqualTo(firstCardId), "Карта має бути такою ж");
            Assert.That(secondResult.IsReversed, Is.EqualTo(firstCardIsReversed), "Орієнтація карти має бути такою ж");
        }

        [Test]
        public async Task GetCardOfTheDayAsync_ReturnsCardWithCorrectReversedState()
        {
            string userId = "user2";

            var firstResult = await _divinationService.GetCardOfTheDayAsync(userId);
            bool firstIsReversed = firstResult.IsReversed;

            var secondResult = await _divinationService.GetCardOfTheDayAsync(userId);

            Assert.That(secondResult.IsReversed, Is.EqualTo(firstIsReversed),
                "Стан карти має залишатися незмінним протягом дня. ");
        }
    }
}