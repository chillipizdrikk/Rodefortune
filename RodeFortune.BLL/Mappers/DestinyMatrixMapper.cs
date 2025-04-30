using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;
using System;

namespace RodeFortune.BLL.Mappers
{
    public static class DestinyMatrixMapper
    {
        public static DestinyMatrixResponseDto ToDto(this DestinyMatrix destinyMatrix)
        {
            if (destinyMatrix == null) return null;

            return new DestinyMatrixResponseDto
            {
                Id = destinyMatrix.Id,
                UserId = destinyMatrix.UserId,
                CreatedAt = destinyMatrix.CreatedAt,
                Content = destinyMatrix.Content
            };
        }

        public static DestinyMatrix ToEntity(this DestinyMatrixRequestDto destinyMatrixDto)
        {
            if (destinyMatrixDto == null) return null;

            return new DestinyMatrix
            {
                UserId = destinyMatrixDto.UserId,
                Content = destinyMatrixDto.Content,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
