using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;

namespace RodeFortune.BLL.Mappers
{
    public static class PostMapper
    {
        public static PostResponseDto ToDto(this Post post)
        {
            if (post == null) return null;

            return new PostResponseDto
            {
                Id = post.Id,
                Author = post.Author,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                Name = post.Name,
                ImageUrl = post.ImageUrl,
                ReferencedReading = post.ReferencedReading,
                ReferencedHoroscope = post.ReferencedHoroscope,
                ReferencedNatalChart = post.ReferencedNatalChart,
                ReferencedDestinyMatrix = post.ReferencedDestinyMatrix,
                Comments = post.Comments
            };
        }

        public static Post ToEntity(this PostRequestDto postRequestDto)
        {
            if (postRequestDto == null) return null;

            return new Post
            {
                Author = postRequestDto.Author,
                Content = postRequestDto.Content,
                Name = postRequestDto.Name,
                ImageUrl = postRequestDto.ImageUrl,
                ReferencedReading = postRequestDto.ReferencedReading,
                ReferencedHoroscope = postRequestDto.ReferencedHoroscope,
                ReferencedNatalChart = postRequestDto.ReferencedNatalChart,
                ReferencedDestinyMatrix = postRequestDto.ReferencedDestinyMatrix,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Comments = new List<ObjectId>()
            };
        }
    }
}
