using Microsoft.AspNetCore.Mvc;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Repositories.Interfaces;
using RodeFortune.PresentationLayer.Models;
using System.Diagnostics;

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
                    PastCard: result.First(x => x.Position == "Минуле").Card,
                    PastIsReversed: result.First(x => x.Position == "Минуле").IsReversed,
                    PresentCard: result.First(x => x.Position == "Теперішнє").Card,
                    PresentIsReversed: result.First(x => x.Position == "Теперішнє").IsReversed,
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

        public async Task<IActionResult> CaseActionResult()
        {
            _logger.LogInformation("Запит на ворожіння Case/Action/Result");
            
            var result = await _divinationService.GetCaseActionResultReadingAsync();
            var model = (
                CaseCard: result.First(x => x.Position == "Ситуація").Card,
                CaseIsReversed: result.First(x => x.Position == "Ситуація").IsReversed,
                ActionCard: result.First(x => x.Position == "Дія").Card,
                ActionIsReversed: result.First(x => x.Position == "Дія").IsReversed,
                ResultCard: result.First(x => x.Position == "Результат").Card,
                ResultIsReversed: result.First(x => x.Position == "Результат").IsReversed
            );

            _logger.LogInformation("Ворожіння Case/Action/Result виконано: {CaseCardName}, {ActionCardName}, {ResultCardName}",
                model.CaseCard.Name, model.ActionCard.Name, model.ResultCard.Name);
            
            return View(model);
        }
        
        public async Task<IActionResult> DreamReview()
        {
            _logger.LogInformation("Запит на ворожіння Аналіз Сновидіння");
            
            var result = await _divinationService.GetDreamReviewReadingAsync();
            var model = (
                SymbolCard: result.First(x => x.Position == "Символ").Card,
                SymbolIsReversed: result.First(x => x.Position == "Символ").IsReversed,
                MeaningCard: result.First(x => x.Position == "Значення").Card,
                MeaningIsReversed: result.First(x => x.Position == "Значення").IsReversed,
                AdviceCard: result.First(x => x.Position == "Порада").Card,
                AdviceIsReversed: result.First(x => x.Position == "Порада").IsReversed
            );
            
            _logger.LogInformation("Ворожіння Аналіз Сновидіння виконано: {SymbolCardName}, {MeaningCardName}, {AdviceCardName}",
                model.SymbolCard.Name, model.MeaningCard.Name, model.AdviceCard.Name);
            
            return View(model);
        }

        public async Task<IActionResult> ProblemSolution()
        {
            _logger.LogInformation("Запит на ворожіння Проблема та Вирішення");
            try
            {
                var result = await _divinationService.GetProblemSolutionReadingAsync();
                var model = (
                    ProblemCard: result.First(x => x.Position == "Проблема").Card,
                    ProblemIsReversed: result.First(x => x.Position == "Проблема").IsReversed,
                    SolutionCard: result.First(x => x.Position == "Вирішення").Card,
                    SolutionIsReversed: result.First(x => x.Position == "Вирішення").IsReversed
                );

                _logger.LogInformation("Ворожіння Проблема та Вирішення виконано: {ProblemCardName}, {SolutionCardName}",
                    model.ProblemCard.Name, model.SolutionCard.Name);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при виконанні ворожіння Проблема та Вирішення");
                return View("Error");
            }
        }
    }
}