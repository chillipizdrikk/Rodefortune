using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;



namespace RodeFortune.UnitTests.Services
{
    [TestFixture]
    public class AdminPanelServiceTests
    {
        private AdminPanelService _adminPanelService;
        private Mock<ITarotCardRepository> _mockTarotCardRepository;
        private Mock<IHoroscopeRepository> _mockHoroscopeRepository;
        private Mock<ILogger<AdminPanelService>> _mockLogger;

        [SetUp]
        public void Setup()
        {
            _mockTarotCardRepository = new Mock<ITarotCardRepository>();
            _mockHoroscopeRepository = new Mock<IHoroscopeRepository>();
            _mockLogger = new Mock<ILogger<AdminPanelService>>();

            _adminPanelService = new AdminPanelService(
                _mockTarotCardRepository.Object,
                _mockHoroscopeRepository.Object,
                _mockLogger.Object
            );
        }

        [Test]
        public async Task CreateTarotCardAsync_ShouldReturnError_WhenNameIsEmpty()
        {
            var result = await _adminPanelService.CreateTarotCardAsync("", "Major", "Motto", "Meaning", false);

            Assert.That(result.Success, Is.False, "Створення карти повинно завершитись помилкою");
            Assert.That(result.Message, Is.EqualTo("Tarot card name  was empty or null"), "Повідомлення про помилку повинно відповідати");
        }

        [Test]
        public async Task CreateTarotCardAsync_ShouldReturnSuccess_WhenValidInput()
        {
            _mockTarotCardRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<TarotCard>()))
                .Returns(Task.CompletedTask);

            var result = await _adminPanelService.CreateTarotCardAsync("The Fool", "Major", "New Beginnings", "Represents a new journey", false);

            Assert.That(result.Success, Is.True, "Створення карти повинно бути успішним");
            Assert.That(result.Data, Is.Not.Null, "Повинна повертатись створена карта");
            Assert.That(result.Data.Name, Is.EqualTo("The Fool"), "Назва карти повинна співпадати");
        }


        [Test]
        public async Task DeleteTarotCardByNameAsync_ShouldReturnError_WhenNameIsEmpty()
        {
            var result = await _adminPanelService.DeleteTarotCardByNameAsync("");

            Assert.That(result.Success, Is.False, "Видалення карти повинно завершитись помилкою");
            Assert.That(result.Message, Is.EqualTo("Tarot card name was empty or null"), "Повідомлення про помилку повинно відповідати");
        }


        [Test]
        public async Task DeleteTarotCardByNameAsync_ShouldReturnSuccess_WhenDeletedSuccessfully()
        {
            var testCard = new TarotCard { Name = "The Fool" };

            _mockTarotCardRepository
                .Setup(repo => repo.GetCardByNameAsync("The Fool"))
                .ReturnsAsync(testCard);

            _mockTarotCardRepository
                .Setup(repo => repo.DeleteByNameAsync("The Fool"))
                .Returns(Task.CompletedTask);

            var result = await _adminPanelService.DeleteTarotCardByNameAsync("The Fool");

            Assert.That(result.Success, Is.True, "Видалення карти повинно бути успішним");
            Assert.That(result.Message, Is.EqualTo("Tarot card deleted successfully"), "Повідомлення повинно бути про успішне видалення");
        }

        [Test]
        public async Task CreateHoroscopeAsync_ShouldReturnError_WhenZodiacSignIsEmpty()
        {
            string zodiacSign = "";
            string motto = "Daily Motto";
            string content = "Your daily horoscope content";
            DateTime date = DateTime.UtcNow.Date;

            var result = await _adminPanelService.CreateHoroscopeAsync(zodiacSign, motto, content, date);

            Assert.That(result.Success, Is.False, "Створення гороскопу повинно завершитись помилкою");
            Assert.That(result.Data, Is.Null, "Дані повинні бути null при помилці");
        }

        [Test]
        public async Task CreateHoroscopeAsync_ShouldReturnSuccess_WhenValidInput()
        {
            // Arrange
            string zodiacSign = "Aries";
            string motto = "Daily Motto";
            string content = "Your daily horoscope content";
            DateTime date = DateTime.UtcNow.Date;

            _mockHoroscopeRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<Horoscope>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _adminPanelService.CreateHoroscopeAsync(zodiacSign, motto, content, date);

            // Assert
            Assert.That(result.Success, Is.True, "Створення гороскопу повинно бути успішним");
            Assert.That(result.Data, Is.Not.Null, "Повинен повертатись створений гороскоп");
            Assert.That(result.Data.ZodiacSign, Is.EqualTo(zodiacSign), "Знак зодіаку повинен співпадати");
            Assert.That(result.Data.Motto, Is.EqualTo(motto), "Девіз повинен співпадати");
            Assert.That(result.Data.Content, Is.EqualTo(content), "Контент повинен співпадати");
            Assert.That(result.Data.Date.Kind, Is.EqualTo(DateTimeKind.Utc), "Тип дати повинен бути UTC");
            Assert.That(result.Data.Date.Date, Is.EqualTo(date.Date), "Дата повинна співпадати");
        }

        [Test]
        public async Task UpdateHoroscopeAsync_ShouldUpdateOnlyProvidedFields()
        {
            // Arrange
            var id = ObjectId.GenerateNewId();
            var existingHoroscope = new Horoscope
            {
                ZodiacSign = "Aries",
                Motto = "Original Motto",
                Content = "Original Content",
                Date = DateTime.SpecifyKind(new DateTime(2023, 1, 1), DateTimeKind.Utc)
            };

            _mockHoroscopeRepository
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(existingHoroscope);

            _mockHoroscopeRepository
                .Setup(repo => repo.UpdateAsync(id, It.IsAny<Horoscope>()))
                .Returns(Task.CompletedTask);

            // Act - only update the motto
            var result = await _adminPanelService.UpdateHoroscopeAsync(id, null, "Updated Motto", null, null);

            // Assert
            Assert.That(result.Success, Is.True, "Оновлення гороскопу повинно бути успішним");
            Assert.That(result.Data, Is.Not.Null, "Повинен повертатись оновлений гороскоп");
            Assert.That(result.Data.ZodiacSign, Is.EqualTo("Aries"), "Знак зодіаку не повинен змінюватись");
            Assert.That(result.Data.Motto, Is.EqualTo("Updated Motto"), "Девіз повинен бути оновленим");
            Assert.That(result.Data.Content, Is.EqualTo("Original Content"), "Контент не повинен змінюватись");
            Assert.That(result.Data.Date, Is.EqualTo(new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)), "Дата не повинна змінюватись");
        }

        [Test]
        public async Task UpdateHoroscopeAsync_ShouldReturnError_WhenHoroscopeNotFound()
        {
            var id = ObjectId.GenerateNewId();
            _mockHoroscopeRepository
               .Setup(repo => repo.GetByIdAsync(id))
               .ReturnsAsync(() => null);

            var result = await _adminPanelService.UpdateHoroscopeAsync(id, "Aries", "Updated Motto", "Updated Content", DateTime.UtcNow);

            Assert.That(result.Success, Is.False, "Оновлення гороскопу повинно завершитись помилкою");
            Assert.That(result.Message, Is.EqualTo("Horoscope not found"), "Повідомлення про помилку повинно відповідати");
            Assert.That(result.Data, Is.Null, "Дані повинні бути null при помилці");
        }

        [Test]
        public async Task DeleteHoroscopeAsync_ShouldReturnError_WhenHoroscopeNotFound()
        {
            var id = ObjectId.GenerateNewId();
            _mockHoroscopeRepository
               .Setup(repo => repo.GetByIdAsync(id))
               .ReturnsAsync(() => null);

            var result = await _adminPanelService.DeleteHoroscopeAsync(id);

            Assert.That(result.Success, Is.False, "Видалення гороскопу повинно завершитись помилкою");
            Assert.That(result.Message, Is.EqualTo("Horoscope not found"), "Повідомлення про помилку повинно відповідати");
            Assert.That(result.Data, Is.False, "Дані повинні показувати неуспішне видалення");
        }

        [Test]
        public async Task DeleteHoroscopeAsync_ShouldReturnSuccess_WhenDeletedSuccessfully()
        {
            var id = ObjectId.GenerateNewId();
            var existingHoroscope = new Horoscope
            {
                ZodiacSign = "Aries",
                Motto = "Daily Motto",
                Content = "Horoscope content"
            };

            _mockHoroscopeRepository
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(existingHoroscope);

            _mockHoroscopeRepository
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            var result = await _adminPanelService.DeleteHoroscopeAsync(id);

            // Assert
            Assert.That(result.Success, Is.True, "Видалення гороскопу повинно бути успішним");
            Assert.That(result.Message, Is.EqualTo("Horoscope deleted successfully"), "Повідомлення повинно бути про успішне видалення");
            Assert.That(result.Data, Is.True, "Дані повинні показувати успішне видалення");
        }

    }
}

