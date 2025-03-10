﻿using MongoDB.Bson;
using System;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class HoroscopeRequestDto
    {
        public string ZodiacSign { get; set; }
        public string Motto { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
    }

    public class HoroscopeResponseDto
    {
        public ObjectId Id { get; set; }
        public string ZodiacSign { get; set; }
        public string Motto { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
    }
}
