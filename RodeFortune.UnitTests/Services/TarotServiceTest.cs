using MongoDB.Driver;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;

namespace RodeFortune.UnitTests.Services
{
    [TestFixture]
    public class TarotServiceTests
    {
        private Mock<IMongoClient> _mockMongoClient;
        private Mock<IMongoDatabase> _mockDatabase;
        private Mock<IMongoCollection<TarotCard>> _mockCollection;
        private Mock<IAsyncCursor<TarotCard>> _mockCursor;
        private TarotService _tarotService;
        private List<TarotCard> _mockTarotCards;

        [SetUp]
        public void Setup()
        {
            _mockMongoClient = new Mock<IMongoClient>();
            _mockDatabase = new Mock<IMongoDatabase>();
            _mockCollection = new Mock<IMongoCollection<TarotCard>>();
            _mockCursor = new Mock<IAsyncCursor<TarotCard>>();

            _mockMongoClient.Setup(client => client.GetDatabase(It.IsAny<string>(), null))
                .Returns(_mockDatabase.Object);

            _mockDatabase.Setup(db => db.GetCollection<TarotCard>(It.IsAny<string>(), null))
                .Returns(_mockCollection.Object);

            _mockTarotCards = new List<TarotCard>
            {
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Маг", Arcana = "Major", Motto = "Я можу все", Meaning = "Сила волі та магія" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Верховна Жриця", Arcana = "Major", Motto = "Інтуїція веде до істини", Meaning = "Інтуїція та таємниці" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Імператриця", Arcana = "Major", Motto = "Все росте з любов'ю", Meaning = "Творчість та достаток" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Імператор", Arcana = "Major", Motto = "Порядок понад усе", Meaning = "Структура та керівництво" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Ієрофант", Arcana = "Major", Motto = "Знання через традиції", Meaning = "Традиції та духовність" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Коханці", Arcana = "Major", Motto = "Вибір серця", Meaning = "Вибір та гармонія" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Колісниця", Arcana = "Major", Motto = "Рушій перемог", Meaning = "Рух та контроль" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Справедливість", Arcana = "Major", Motto = "Правда веде до гармонії", Meaning = "Справедливість, істина, рівновага" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Відлюдник", Arcana = "Major", Motto = "Світло серед темряви", Meaning = "Пошук істини, самопізнання" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Колесо Фортуни", Arcana = "Major", Motto = "Прийми зміни", Meaning = "Доля, удача, цикл життя" },
                new TarotCard { Id = ObjectId.GenerateNewId(), Name = "Сила", Arcana = "Major", Motto = "М'яка сила перемагає", Meaning = "Внутрішня сила, терпіння" }
            };

            _mockCursor.Setup(cursor => cursor.Current).Returns(_mockTarotCards);
            _mockCursor.SetupSequence(cursor => cursor.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            _mockCursor.SetupSequence(cursor => cursor.MoveNextAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true))
                .Returns(Task.FromResult(false));

            _mockCollection.Setup(collection => collection
                .FindAsync(It.IsAny<FilterDefinition<TarotCard>>(), It.IsAny<FindOptions<TarotCard>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(_mockCursor.Object);

            _tarotService = new TarotService(_mockMongoClient.Object);
        }


        [Test]
        public async Task GetTarotCardByBirthDateAsync_OddDay_ReturnsReversedCard()
        {
            var birthDate = new DateTime(1990, 5, 13);

            var result = await _tarotService.GetTarotCardByBirthDateAsync(birthDate);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Reversal, Is.True); //  Пояснення: дата 13.05.1990 - для непарного дня карта має бути перевернутою
        }

        [Test]
        public async Task GetTarotCardByBirthDateAsync_EvenDay_ReturnsNotReversedCard()
        {
            var birthDate = new DateTime(1990, 5, 14);

            var result = await _tarotService.GetTarotCardByBirthDateAsync(birthDate);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Reversal, Is.False); // Те саме, але 14 - парна дата, отже карта не повинна бути перевернутою
        }

        [Test]
        public void CalculateTarotNumber_ReturnsCorrectNumber()
        {
            var birthDate = new DateTime(1988, 12, 25);  // 2+5+1+2+1+9+8+8 = 36, 3+6 = 9

            var result = (int)typeof(TarotService)
                .GetMethod("CalculateTarotNumber", BindingFlags.NonPublic | BindingFlags.Instance)
                ?.Invoke(_tarotService, new object[] { birthDate });

            Assert.That(result, Is.EqualTo(9));
        }


    }
}