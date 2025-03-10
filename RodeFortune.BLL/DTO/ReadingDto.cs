﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class ReadingRequestDto
    {
        [Required]
        public ObjectId AuthorId { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public List<ObjectId> Cards { get; set; }
    }

    public class ReadingResponseDto
    {
        public ObjectId Id { get; set; }
        public ObjectId AuthorId { get; set; }
        public string Type { get; set; }
        public List<ObjectId> Cards { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
