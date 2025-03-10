using System;

namespace RodeFortune.BLL.Enums
{
    public enum ZodiacSign
    {
        Aries,        // Овен (21 березня – 19 квітня)
        Taurus,       // Телець (20 квітня – 20 травня)
        Gemini,       // Близнюки (21 травня – 20 червня)
        Cancer,       // Рак (21 червня – 22 липня)
        Leo,          // Лев (23 липня – 22 серпня)
        Virgo,        // Діва (23 серпня – 22 вересня)
        Libra,        // Терези (23 вересня – 22 жовтня)
        Scorpio,      // Скорпіон (23 жовтня – 21 листопада)
        Sagittarius,  // Стрілець (22 листопада – 21 грудня)
        Capricorn,    // Козеріг (22 грудня – 19 січня)
        Aquarius,     // Водолій (20 січня – 18 лютого)
        Pisces        // Риби (19 лютого – 20 березня)
    }

    public static class ZodiacHelper
    {
        public static ZodiacSign GetZodiacSign(DateTime date)
        {
            int day = date.Day;
            int month = date.Month;

            return month switch
            {
                1 => (day <= 19) ? ZodiacSign.Capricorn : ZodiacSign.Aquarius,
                2 => (day <= 18) ? ZodiacSign.Aquarius : ZodiacSign.Pisces,
                3 => (day <= 20) ? ZodiacSign.Pisces : ZodiacSign.Aries,
                4 => (day <= 19) ? ZodiacSign.Aries : ZodiacSign.Taurus,
                5 => (day <= 20) ? ZodiacSign.Taurus : ZodiacSign.Gemini,
                6 => (day <= 20) ? ZodiacSign.Gemini : ZodiacSign.Cancer,
                7 => (day <= 22) ? ZodiacSign.Cancer : ZodiacSign.Leo,
                8 => (day <= 22) ? ZodiacSign.Leo : ZodiacSign.Virgo,
                9 => (day <= 22) ? ZodiacSign.Virgo : ZodiacSign.Libra,
                10 => (day <= 22) ? ZodiacSign.Libra : ZodiacSign.Scorpio,
                11 => (day <= 21) ? ZodiacSign.Scorpio : ZodiacSign.Sagittarius,
                12 => (day <= 21) ? ZodiacSign.Sagittarius : ZodiacSign.Capricorn,
                _ => throw new ArgumentOutOfRangeException(nameof(date), "Некоректна дата")
            };
        }
    }
}
