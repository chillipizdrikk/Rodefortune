using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class NatalChartRequestDto
    {
        [Required]
        public ObjectId UserId { get; set; }

        [Required]
        public List<ObjectId> Houses { get; set; }

        public byte[] ImageUrl { get; set; }
    }

    public class NatalChartResponseDto
    {
        public ObjectId Id { get; set; }
        public ObjectId UserId { get; set; }
        public List<ObjectId> Houses { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] ImageUrl { get; set; }
    }
}
