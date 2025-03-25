using Microsoft.Extensions.Logging;
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

    }
}

