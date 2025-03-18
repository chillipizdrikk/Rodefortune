using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RodeFortune.BLL.Dto;
using RodeFortune.BLL.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace RodeFortune.PresentationLayer.Controllers
{
    public class HoroscopeController : Controller
    {
        private readonly IHoroscopeService _horoscopeService;
        private readonly ILogger<HoroscopeController> _logger;

        public HoroscopeController(IHoroscopeService horoscopeService, ILogger<HoroscopeController> logger)
        {
            _horoscopeService = horoscopeService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(DateTime? date = null)
        {
            _logger.LogInformation("Відображення сторінки гороскопів за датою: {Date}", date?.ToShortDateString() ?? "поточна дата");
            
            try
            {
                var targetDate = date ?? DateTime.Today;
                ViewBag.SelectedDate = targetDate;
                
                var horoscopes = await _horoscopeService.GetHoroscopesByDateAsync(targetDate);
                return View(horoscopes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гороскопів для дати: {Date}", date?.ToShortDateString() ?? "поточна дата");
                return View("Error");
            }
        }

        public async Task<IActionResult> BySign(string zodiacSign)
        {
            if (string.IsNullOrEmpty(zodiacSign))
            {
                return RedirectToAction("Index");
            }

            _logger.LogInformation("Відображення гороскопів для знаку: {ZodiacSign}", zodiacSign);
            
            try
            {
                var horoscopes = await _horoscopeService.GetHoroscopesByZodiacSignAsync(zodiacSign);
                ViewBag.ZodiacSign = zodiacSign;
                return View(horoscopes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гороскопів для знаку: {ZodiacSign}", zodiacSign);
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("Відображення форми створення гороскопу");
            ViewBag.ZodiacSigns = new[]
            {
                "Овен", "Телець", "Близнюки", "Рак", "Лев", "Діва",
                "Терези", "Скорпіон", "Стрілець", "Козеріг", "Водолій", "Риби"
            };
            return View(new HoroscopeRequestDto { Date = DateTime.Today });
        }

        [HttpPost]
        public async Task<IActionResult> Create(HoroscopeRequestDto horoscopeDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Помилка валідації при створенні гороскопу");
                ViewBag.ZodiacSigns = new[]
                {
                    "Овен", "Телець", "Близнюки", "Рак", "Лев", "Діва",
                    "Терези", "Скорпіон", "Стрілець", "Козеріг", "Водолій", "Риби"
                };
                return View(horoscopeDto);
            }

            _logger.LogInformation("Створення нового гороскопу для знаку: {ZodiacSign}", horoscopeDto.ZodiacSign);
            
            try
            {
                await _horoscopeService.CreateHoroscopeAsync(horoscopeDto);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при створенні гороскопу для знаку: {ZodiacSign}", horoscopeDto.ZodiacSign);
                ModelState.AddModelError("", "Сталася помилка при збереженні гороскопу.");
                ViewBag.ZodiacSigns = new[]
                {
                    "Овен", "Телець", "Близнюки", "Рак", "Лев", "Діва",
                    "Терези", "Скорпіон", "Стрілець", "Козеріг", "Водолій", "Риби"
                };
                return View(horoscopeDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Редагування гороскопу з ID: {Id}", id);
            
            try
            {
                var horoscope = await _horoscopeService.GetHoroscopeByIdAsync(id);
                if (horoscope == null)
                {
                    return NotFound();
                }

                ViewBag.ZodiacSigns = new[]
                {
                    "Овен", "Телець", "Близнюки", "Рак", "Лев", "Діва",
                    "Терези", "Скорпіон", "Стрілець", "Козеріг", "Водолій", "Риби"
                };

                var requestDto = new HoroscopeRequestDto
                {
                    ZodiacSign = horoscope.ZodiacSign,
                    Motto = horoscope.Motto,
                    Content = horoscope.Content,
                    Date = horoscope.Date
                };

                ViewBag.HoroscopeId = id;
                return View(requestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні гороскопу з ID: {Id} для редагування", id);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, HoroscopeRequestDto horoscopeDto)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Помилка валідації при оновленні гороскопу з ID: {Id}", id);
                ViewBag.ZodiacSigns = new[]
                {
                    "Овен", "Телець", "Близнюки", "Рак", "Лев", "Діва",
                    "Терези", "Скорпіон", "Стрілець", "Козеріг", "Водолій", "Риби"
                };
                ViewBag.HoroscopeId = id;
                return View(horoscopeDto);
            }

            _logger.LogInformation("Оновлення гороскопу з ID: {Id}", id);
            
            try
            {
                var result = await _horoscopeService.UpdateHoroscopeAsync(id, horoscopeDto);
                if (result == null)
                {
                    return NotFound();
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при оновленні гороскопу з ID: {Id}", id);
                ModelState.AddModelError("", "Сталася помилка при оновленні гороскопу.");
                ViewBag.ZodiacSigns = new[]
                {
                    "Овен", "Телець", "Близнюки", "Рак", "Лев", "Діва",
                    "Терези", "Скорпіон", "Стрілець", "Козеріг", "Водолій", "Риби"
                };
                ViewBag.HoroscopeId = id;
                return View(horoscopeDto);
            }
        }

        [HttpGet]
        [Route("Horoscope/GetHoroscopeDetails/{id}")]
        public async Task<IActionResult> GetHoroscopeDetails(string id)
        {
            try
            {
                _logger.LogInformation("Запит на деталі гороскопу з ID: {Id}", id);
                
                var horoscope = await _horoscopeService.GetHoroscopeByIdAsync(id);
                if (horoscope == null)
                {
                    _logger.LogWarning("Гороскоп з ID {Id} не знайдено", id);
                    return NotFound();
                }

                var horoscopeDetails = new
                {
                    id = horoscope.Id.ToString(),
                    zodiacSign = horoscope.ZodiacSign,
                    motto = horoscope.Motto,
                    content = horoscope.Content,
                    date = horoscope.Date.ToString("yyyy-MM-dd")
                };

                return Json(horoscopeDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при отриманні деталей гороскопу з ID: {Id}", id);
                return StatusCode(500, "Внутрішня помилка сервера");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            _logger.LogInformation("Видалення гороскопу з ID: {Id}", id);
            
            try
            {
                await _horoscopeService.DeleteHoroscopeAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Помилка при видаленні гороскопу з ID: {Id}", id);
                return View("Error");
            }
        }
    }
}