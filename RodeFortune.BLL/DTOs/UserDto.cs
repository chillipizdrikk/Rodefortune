using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RodeFortune.BLL.Dto
{
    public class UserRequestDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime BirthDate { get; set; }
        public string ZodiacSign { get; set; }
        public string Role { get; set; }
    }

    public class UserResponseDto
    {
        public ObjectId Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }
        public string ZodiacSign { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public byte[] Avatar { get; set; }
        public List<ObjectId> SavedReadings { get; set; }
        public List<ObjectId> SavedHoroscopes { get; set; }
        public ObjectId? NatalChart { get; set; }
        public ObjectId? DestinyMatrix { get; set; }
    }
}
