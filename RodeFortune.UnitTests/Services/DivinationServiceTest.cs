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

        [SetUp]
        public void Setup()
        {
            _mockTarotCardRepository = new Mock<ITarotCardRepository>();
            _mockLogger = new Mock<ILogger<DivinationService>>();

            _testCards = new List<TarotCard>
            {
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Колесо Фортуни", Arcana = "Major" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Маг", Arcana = "Major" }
            };

            _mockTarotCardRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(_testCards);
            _divinationService = new DivinationService(_mockTarotCardRepository.Object, _mockLogger.Object);

        }

        [Test]
        public async Task GetYesNoReadingAsync_ShouldReturnValidCard()
        {
            var result = await _divinationService.GetYesNoReadingAsync();
            Assert.That(result.Card, Is.Not.Null, "Повинна повертатись карта");
            Assert.That(result.Card.Name, Is.AnyOf("Колесо Фортуни", "Маг"),
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

    }
}