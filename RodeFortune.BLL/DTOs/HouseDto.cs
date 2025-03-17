﻿using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class HouseRequestDto
    {
        public string HouseNumber { get; set; }
        public string Sign { get; set; }
        public string Planet { get; set; }
        public string Content { get; set; }
    }

    public class HouseResponseDto
    {
        public ObjectId Id { get; set; }
        public string HouseNumber { get; set; }
        public string Sign { get; set; }
        public string Planet { get; set; }
        public string Content { get; set; }
    }
}
