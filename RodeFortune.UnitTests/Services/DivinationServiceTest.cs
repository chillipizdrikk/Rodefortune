using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using NUnit.Framework;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Models;
using Microsoft.Extensions.Logging;

namespace RodeFortune.Tests.BLL.Services
{
    [TestFixture]
    public class DivisionServiceTests
    {
        private DivinationService _divisionService;
        private Mock<IMongoDatabase> _mockDatabase;
        private Mock<IMongoCollection<TarotCard>> _mockCollection;
        private Mock<ILogger<DivinationService>> _mockLogger;
        private List<TarotCard> _testCards;

        [SetUp]
        public void Setup()
        {
            _testCards = new List<TarotCard>
            {
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Колесо Фортуни", Arcana = "Major" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Маг", Arcana = "Major" }
            };

            _mockCollection = new Mock<IMongoCollection<TarotCard>>();
            var mockCursor = new Mock<IAsyncCursor<TarotCard>>();
            mockCursor.Setup(c => c.Current).Returns(_testCards);
            mockCursor
                .SetupSequence(c => c.MoveNextAsync(default))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection
                .Setup(c => c.FindAsync(
                    It.IsAny<FilterDefinition<TarotCard>>(),
                    It.IsAny<FindOptions<TarotCard, TarotCard>>(),
                    default))
                .ReturnsAsync(mockCursor.Object);

            _mockDatabase = new Mock<IMongoDatabase>();
            _mockDatabase
                .Setup(d => d.GetCollection<TarotCard>("TarotCards", null))
                .Returns(_mockCollection.Object);

            // Додаємо мок для ILogger
            _mockLogger = new Mock<ILogger<DivinationService>>();

            // Створюємо екземпляр сервісу з моками бази даних і логера
            _divisionService = new DivinationService(_mockDatabase.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetYesNoReadingAsync_ShouldReturnValidCard()
        {
            var result = await _divisionService.GetYesNoReadingAsync();
            Assert.That(result.Card, Is.Not.Null, "Повинна повертатись карта");
            Assert.That(result.Card.Name, Is.AnyOf("Колесо Фортуни", "Маг"),
                "Назва карти повинна бути однією з тестових карт");
        }

        [Test]
        public async Task GetYesNoReadingAsync_ShouldReturnCardWithCorrectProperties()
        {
            var result = await _divisionService.GetYesNoReadingAsync();
            Assert.That(result.Card, Is.Not.Null, "Повинна повертатись карта");
            Assert.That(result.Card.Arcana, Is.EqualTo("Major"),
                "Карта повинна належати до Старших Арканів");
            Assert.That(result.IsReversed, Is.AnyOf(true, false),
                "Карта має бути перевернутою або ні");
        }
    }
}