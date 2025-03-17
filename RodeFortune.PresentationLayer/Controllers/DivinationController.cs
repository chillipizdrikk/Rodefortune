using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RodeFortune.PresentationLayer.Controllers
{
    public class DivinationController : Controller
    {
        private readonly DivinationService _divinationService;
        private readonly ILogger<DivinationController> _logger;

        public DivinationController(DivinationService divinationService, ILogger<DivinationController> logger)
        {
            _divinationService = divinationService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Відображення головної сторінки ворожінь");
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
    }
}