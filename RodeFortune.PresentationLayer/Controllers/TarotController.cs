using Microsoft.AspNetCore.Mvc;
using RodeFortune.BLL.Services.Interfaces;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace RodeFortune.PresentationLayer.Controllers
{
    public class TarotController : Controller
    {
        private readonly IConstantDivinationService _iconstantDivinationService;

        public TarotController(IConstantDivinationService constantDivinationService)
        {
            _iconstantDivinationService = constantDivinationService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            return RedirectToAction("BirthDateTarot");
        }

       [HttpGet]
       [Authorize]
        public IActionResult BirthDateTarot()
        {
            ViewBag.ShowResult = false;
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BirthDateTarot(DateTime birthDate)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ShowResult = false;
                return View();
            }

            var tarotCard = await _iconstantDivinationService.GetTarotCardByBirthDateAsync(birthDate);
            string personalizedMessage = GeneratePersonalizedMessage(tarotCard, birthDate);

            ViewBag.BirthDate = birthDate;
            ViewBag.PersonalizedMessage = personalizedMessage;
            ViewBag.ShowResult = true;

            return View(tarotCard);
        }
        [Authorize]
        private string GeneratePersonalizedMessage(TarotCard card, DateTime birthDate)
        {
            string zodiacSign = GetZodiacSign(birthDate);

            return $"Ваша карта Таро '{card.Name}' відображає сутність вашої особистості. " +
                   $"Як людина зі знаком {zodiacSign}, ви маєте особливий зв'язок з цією картою. " +
                   $"{(string.IsNullOrEmpty(card.Motto) ? "" : $"'{card.Motto}' - це принцип, який супроводжує вас у житті.")} ";
        }
        [Authorize]
        private string GetZodiacSign(DateTime birthDate)
        {
            int day = birthDate.Day;
            int month = birthDate.Month;

            switch (month)
            {
                case 1: return day <= 19 ? "Козеріг" : "Водолій";
                case 2: return day <= 18 ? "Водолій" : "Риби";
                case 3: return day <= 20 ? "Риби" : "Овен";
                case 4: return day <= 19 ? "Овен" : "Телець";
                case 5: return day <= 20 ? "Телець" : "Близнюки";
                case 6: return day <= 20 ? "Близнюки" : "Рак";
                case 7: return day <= 22 ? "Рак" : "Лев";
                case 8: return day <= 22 ? "Лев" : "Діва";
                case 9: return day <= 22 ? "Діва" : "Терези";
                case 10: return day <= 22 ? "Терези" : "Скорпіон";
                case 11: return day <= 21 ? "Скорпіон" : "Стрілець";
                case 12: return day <= 21 ? "Стрілець" : "Козеріг";
                default: return "Невідомий знак";
            }
        }
    }
}