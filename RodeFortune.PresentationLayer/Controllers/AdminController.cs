using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.BLL.Services.Interfaces;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using RodeFortune.PresentationLayer.Models;
using System.Diagnostics;

namespace RodeFortune.PresentationLayer.Controllers
{
    public class AdminController : Controller
    {
        private readonly ITarotCardRepository _tarotRepository;
        private readonly IHoroscopeRepository _horoscopeRepository;
        private readonly AdminPanelService _adminPanelService;

        public AdminController(ITarotCardRepository tarotRepository, IHoroscopeRepository horoscopeRepository,
            AdminPanelService adminPanelService)
        {
            _tarotRepository = tarotRepository;
            _horoscopeRepository = horoscopeRepository;
            _adminPanelService = adminPanelService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> TarotCards()
        {
            var divinations = await _tarotRepository.GetAllAsync();
            var divinationViewModels = divinations.Select(tc => new TarotCardViewModel
            {
                Name = tc.Name,
                Motto = tc.Motto,
                Arcana = tc.Arcana,
                Meaning = tc.Meaning,
                Reversal = tc.Reversal,
            }).ToList();

            return View(divinationViewModels);
        }

        public async Task<IActionResult> Horoscopes()
        {
            var horoscopes = await _horoscopeRepository.GetAllAsync();
            var horoscopeViewModels = horoscopes.Select(hr => new CreateHoroscopeViewModel
            {
               Id = hr.Id.ToString(),
               ZodiacSign = hr.ZodiacSign,
               Motto = hr.Motto,
               Content = hr.Content,
               Date = hr.Date
            }).ToList();

            return View(horoscopeViewModels);
        }

        [HttpGet]
        public IActionResult CreateTarotCard()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTarotCard(TarotCardViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                byte[] imageData = null;

                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        imageData = memoryStream.ToArray();
                    }
                }

                var result = await _adminPanelService.CreateTarotCardAsync(
                    model.Name,
                    model.Arcana,
                    model.Motto,
                    model.Meaning,
                    model.Reversal,
                    imageData
                );

                if (result.Success)
                {
                    return RedirectToAction("TarotCards");
                }
                else
                {
                    ModelState.AddModelError("", result.Message);
                }
            }

            return View(model);
        }



        [HttpGet]
        public IActionResult CreateHoroscope()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHoroscope(CreateHoroscopeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _adminPanelService.CreateHoroscopeAsync(
                model.ZodiacSign,
                model.Motto,
                model.Content,
            model.Date);

            if (result.Success)
            {
                return RedirectToAction("Horoscopes");
            }
            else
            {
                ModelState.AddModelError("", result.Message);
            }
            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> UpdateHoroscope(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Hocoscopes");
            }

            var horoscope = await _horoscopeRepository.GetByIdAsync(ObjectId.Parse(id));

            if (horoscope == null)
            {
                return RedirectToAction("Hocoscopes");

            }

            var model = new UpdateHoroscopeViewModel
            {
                Id = id,
                ZodiacSign = horoscope.ZodiacSign,
                Motto = horoscope.Motto,
                Content = horoscope.Content,
                Date = horoscope.Date
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHoroscope(UpdateHoroscopeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _adminPanelService.UpdateHoroscopeAsync(
                ObjectId.Parse(model.Id),
                model.ZodiacSign,
                model.Motto,
                model.Content,
                model.Date
            );

            if (result.Success)
            {
                return RedirectToAction("Horoscopes");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHoroscope(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("Index");
            }

            var result = await _adminPanelService.DeleteHoroscopeAsync(ObjectId.Parse(id));

            if (result.Success)
            {
                return RedirectToAction("Horoscopes");
            }
            else
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                });
            }
        }


        [HttpPost]
        public async Task<IActionResult> DeleteTarotCard(string tarotCardName)
        {
            if (string.IsNullOrWhiteSpace(tarotCardName))
            {
                return RedirectToAction("Index");
            }

            var result = await _adminPanelService.DeleteTarotCardByNameAsync(tarotCardName);

            if (result.Success)
            {
                return RedirectToAction("TarotCards");
            }
            else
            {
                return View("Error", new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateTarotCard(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction("TarotCards");
            }
            
            var existingCard = await _tarotRepository.GetCardByNameAsync(id);

            if (existingCard == null)
            {
                return RedirectToAction("TarotCards");
            }

            var model = new TarotCardViewModel
            {
                Name = existingCard.Name,
                Arcana = existingCard.Arcana,
                Motto = existingCard.Motto,
                Meaning = existingCard.Meaning,
                Reversal = existingCard.Reversal
            };

            return View(model);

        }
    }
}
