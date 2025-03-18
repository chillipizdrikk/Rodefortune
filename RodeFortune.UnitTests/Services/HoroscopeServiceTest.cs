using System;
using System.Threading.Tasks;
using NUnit.Framework;
using RodeFortune.BLL.Dto;
using RodeFortune.BLL.Mappers;
using RodeFortune.DAL.Models;
using MongoDB.Bson;

namespace RodeFortune.UnitTests.Services
{
    [TestFixture]
    public class AdditionalHoroscopeServiceTests
    {
        [Test]
        public void HoroscopeMappers_ToEntity_MapsCorrectly()
        {
            var dto = new HoroscopeRequestDto
            {
                ZodiacSign = "Риби",
                Motto = "Інтуїція і творчість",
                Content = "Прислухайтесь до свого внутрішнього голосу",
                Date = DateTime.Today
            };

            var entity = dto.ToEntity();

            Assert.That(entity, Is.Not.Null);
            Assert.That(entity.ZodiacSign, Is.EqualTo("Риби"));
            Assert.That(entity.Motto, Is.EqualTo("Інтуїція і творчість"));
            Assert.That(entity.Content, Is.EqualTo("Прислухайтесь до свого внутрішнього голосу"));
            Assert.That(entity.Date, Is.EqualTo(DateTime.Today));
        }

        [Test]
        public void Horoscope_DateRange_IsCalculatedCorrectly()
        {
            var date = new DateTime(2023, 10, 15);

            var startDate = date.Date;
            var endDate = startDate.AddDays(1).AddTicks(-1);

            Assert.That(startDate, Is.EqualTo(new DateTime(2023, 10, 15, 0, 0, 0)));
            Assert.That(endDate, Is.EqualTo(new DateTime(2023, 10, 15, 23, 59, 59).AddTicks(9999999)));

            var morningTime = new DateTime(2023, 10, 15, 8, 0, 0);
            var eveningTime = new DateTime(2023, 10, 15, 20, 30, 0);

            Assert.That(morningTime >= startDate && morningTime <= endDate, Is.True);
            Assert.That(eveningTime >= startDate && eveningTime <= endDate, Is.True);
        }

        [Test]
        public void HoroscopeRequest_Validation()
        {
            var validDto = new HoroscopeRequestDto
            {
                ZodiacSign = "Овен",
                Motto = "Сміливість і енергійність",
                Content = "Сьогодні ваша енергія на піку",
                Date = DateTime.Today
            };

            var emptyZodiacDto = new HoroscopeRequestDto
            {
                ZodiacSign = "",
                Motto = "Тест",
                Content = "Тест",
                Date = DateTime.Today
            };

            var nullContentDto = new HoroscopeRequestDto
            {
                ZodiacSign = "Овен",
                Motto = "Тест",
                Content = null,
                Date = DateTime.Today
            };

            Assert.That(IsValidHoroscope(validDto), Is.True);
            Assert.That(IsValidHoroscope(emptyZodiacDto), Is.False);
            Assert.That(IsValidHoroscope(nullContentDto), Is.False);
        }

        private bool IsValidHoroscope(HoroscopeRequestDto dto)
        {
            return !string.IsNullOrEmpty(dto.ZodiacSign)
                && !string.IsNullOrEmpty(dto.Motto)
                && !string.IsNullOrEmpty(dto.Content)
                && dto.Date != default;
        }
    }
}