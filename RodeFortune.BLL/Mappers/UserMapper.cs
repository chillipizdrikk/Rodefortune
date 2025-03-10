using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;

namespace RodeFortune.BLL.Mappers
{
    public static class UserMapper
    {
        public static UserResponseDto ToDto(this User user)
        {
            if (user == null) return null;

            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                BirthDate = user.BirthDate,
                ZodiacSign = user.ZodiacSign,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                Avatar = user.Avatar,
                SavedReadings = user.SavedReadings,
                SavedHoroscopes = user.SavedHoroscopes,
                NatalChart = user.NatalChart,
                DestinyMatrix = user.DestinyMatrix
            };
        }

        public static User ToModel(this UserRequestDto userRequestDto)
        {
            if (userRequestDto == null) return null;

            return new User
            {
                Username = userRequestDto.Username,
                Email = userRequestDto.Email,
                PasswordHash = userRequestDto.PasswordHash,
                BirthDate = userRequestDto.BirthDate,
                ZodiacSign = userRequestDto.ZodiacSign,
                Role = userRequestDto.Role,
                CreatedAt = DateTime.UtcNow,
            };
        }
    }
}