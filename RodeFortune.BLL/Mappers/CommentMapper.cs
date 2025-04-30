using MongoDB.Bson;
using RodeFortune.BLL.Dto;
using RodeFortune.DAL.Models;
using System;

namespace RodeFortune.BLL.Mappers
{
    public static class CommentMapper
    {
        public static CommentResponseDto ToDto(this Comment comment)
        {
            if (comment == null) return null;

            return new CommentResponseDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                AuthorId = comment.AuthorId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt
            };
        }

        public static Comment ToEntity(this CommentRequestDto commentDto)
        {
            if (commentDto == null) return null;

            return new Comment
            {
                PostId = commentDto.PostId,
                AuthorId = commentDto.AuthorId,
                Content = commentDto.Content,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
