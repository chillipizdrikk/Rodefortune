using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using RodeFortune.PresentationLayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace RodeFortune.PresentationLayer.Controllers
{
    public class DivinationController : Controller
    {
        private readonly ITarotCardRepository _tarotCardRepository;
        private readonly DivinationService _divinationService;
        private readonly ILogger<DivinationController> _logger;

        public DivinationController(DivinationService divinationService, ILogger<DivinationController> logger, ITarotCardRepository tarotCardRepository)
        {
            _divinationService = divinationService;
            _tarotCardRepository = tarotCardRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> YesNo()
        {
            _logger.LogInformation("Запит на Yes/No ворожіння");
            try
            {
                var result = await _divinationService.GetYesNoReadingAsync();
                _logger.LogInformation("Yes/No ворожіння виконано: {CardName} {Orientation}",
                    result.Card.Name, result.IsReversed ? "перевернута" : "пряма");
                return View(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при виконанні Yes/No ворожіння");
                return View("Error");
            }
        }

        public async Task<IActionResult> Cards(string? searchTerm = null, string? arcana = null)
        {
            try
            {
                _logger.LogInformation("Запит на перегляд карт з фільтрами: {SearchTerm}, {Arcana}",
                    searchTerm ?? "не вказано", arcana ?? "не вказано");

                var cards = await _divinationService.GetCardsAsync(searchTerm, arcana);

                ViewBag.SearchTerm = searchTerm;
                ViewBag.Arcana = arcana;

                // Get unique arcana values for filter dropdown
                var allCards = await _divinationService.GetCardsAsync();
                var arcanaOptions = allCards
                    .Select(c => c.Arcana)
                    .Where(a => !string.IsNullOrEmpty(a))
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();

                ViewBag.ArcanaOptions = arcanaOptions;

                _logger.LogInformation("Знайдено {Count} карт", cards.Count);
                return View(cards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні списку карт");
                return View("Error");
            }
        }

        [HttpGet("Divination/GetCardDetails/{id}")]
        public async Task<IActionResult> GetCardDetails(string id)
        {
            try
            {
                _logger.LogInformation("Запит на деталі карти з ID: {Id}", id);

                var card = await _tarotCardRepository.GetCardByIdAsync(id);
                if (card == null)
                {
                    _logger.LogWarning("Карта з ID {Id} не знайдена", id);
                    return NotFound();
                }

                var cardDetails = new
                {
                    id = card.Id.ToString(),
                    name = card.Name,
                    arcana = card.Arcana,
                    motto = card.Motto,
                    meaning = card.Meaning,
                    reversal = card.Reversal,
                    imageUrl = card.ImageUrl != null ? Convert.ToBase64String(card.ImageUrl) : null
                };

                return Json(cardDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні деталей карти з ID: {Id}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        //Приклад методу без try-catch, але з Result.  
        public async Task<IActionResult> CardOfTheDay()
        {
            _logger.LogInformation("Запит на ворожіння Карта Дня");
            string userId = "user";

            var resultObject = await _divinationService.GetCardOfTheDayAsync(userId);

            if (!resultObject.Success)
            {
                _logger.LogWarning("Помилка при отриманні Карти Дня: {ErrorMessage}", resultObject.Message);
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                });
            }

            var cardResult = resultObject.Data;

            _logger.LogInformation("Карта Дня ворожіння виконано: {CardName} {Orientation}",
                cardResult.Card.Name, cardResult.IsReversed ? "перевернута" : "пряма");

            return View((cardResult.Card, cardResult.IsReversed));
        }

        public async Task<IActionResult> PresentPastFuture()
        {
            _logger.LogInformation("Запит на ворожіння Present/Past/Future");
            try
            {
                var result = await _divinationService.GetPastPresentFutureReadingAsync();
                var model = (
                    PresentCard: result.First(x => x.Position == "Теперішнє").Card,
                    PresentIsReversed: result.First(x => x.Position == "Теперішнє").IsReversed,
                    PastCard: result.First(x => x.Position == "Минуле").Card,
                    PastIsReversed: result.First(x => x.Position == "Минуле").IsReversed,
                    FutureCard: result.First(x => x.Position == "Майбутнє").Card,
                    FutureIsReversed: result.First(x => x.Position == "Майбутнє").IsReversed
                );

                _logger.LogInformation("Ворожіння Present/Past/Future виконано: {PresentCardName}, {PastCardName}, {FutureCardName}",
                    model.PresentCard.Name, model.PastCard.Name, model.FutureCard.Name);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при виконанні ворожіння Present/Past/Future");
                return View("Error");
            }
        }
    }
}