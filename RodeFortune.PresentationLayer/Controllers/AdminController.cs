using Microsoft.AspNetCore.Mvc;
using RodeFortune.BLL.Services.Implementations;
using RodeFortune.DAL.Repositories.Interfaces;
using RodeFortune.PresentationLayer.Models;

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

        [HttpPost]
        public async Task<IActionResult> DeleteTarotCard(string tarotCardName)
        {
            if (string.IsNullOrWhiteSpace(tarotCardName))
            {
                TempData["ErrorMessage"] = "Tarot card name cannot be empty.";
                return RedirectToAction("Index");
            }

            var result = await _adminPanelService.DeleteTarotCardByNameAsync(tarotCardName);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["SuccessMessage"] = "Tarot card deleted successfully!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditTarotCard(string id)
        {
            var existingCard = await _tarotRepository.GetCardByNameAsync(id);

            if (existingCard == null)
            {
                TempData["ErrorMessage"] = "The card was not found.";
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















        //[HttpPost]
        //public async Task<IActionResult> EditTarotCard(string tarotCardName, TarotCardViewModel model, IFormFile? imageFile)
        //{
        //    if (string.IsNullOrWhiteSpace(tarotCardName))
        //    {
        //        TempData["ErrorMessage"] = "Tarot card name cannot be empty.";
        //        return RedirectToAction("Index");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.Values
        //            .SelectMany(v => v.Errors)
        //            .Select(e => e.ErrorMessage);

        //        return View(model); 
        //    }

        //    ModelState.Clear();
        //    byte[]? imageData = null;
        //    if (imageFile != null)
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            await imageFile.CopyToAsync(memoryStream);
        //            imageData = memoryStream.ToArray();
        //        }
        //    }



        //    var result = await _adminPanelService.EditTarotCardAsync(
        //        tarotCardName,
        //        model.Name,
        //        model.Arcana,
        //        model.Motto,
        //        model.Meaning,
        //        model.Reversal,
        //        imageData);

        //    if (!result.Success)
        //    {
        //        TempData["ErrorMessage"] = result.Message;
        //        return View(model);
        //    }

        //    TempData["SuccessMessage"] = "Tarot card edited successfully!";
        //    return RedirectToAction("Index");
        //}

    }
}
