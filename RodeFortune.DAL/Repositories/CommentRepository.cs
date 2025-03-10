using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories
{
    public class CommentRepository
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentRepository(IMongoDatabase database)
        {
            _comments = database.GetCollection<Comment>("comments");
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _comments.Find(_ => true).ToListAsync();
        }

        public async Task<Comment> GetByIdAsync(ObjectId id)
        {
            return await _comments.Find(comment => comment.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Comment>> GetByPostIdAsync(ObjectId postId)
        {
            return await _comments.Find(comment => comment.PostId == postId).ToListAsync();
        }

        public async Task CreateAsync(Comment comment)
        {
            comment.CreatedAt = DateTime.UtcNow;
            comment.UpdatedAt = DateTime.UtcNow;
            await _comments.InsertOneAsync(comment);
        }

        public async Task<bool> UpdateAsync(Comment comment)
        {
            var update = Builders<Comment>.Update
                .Set(c => c.Content, comment.Content)
                .Set(c => c.UpdatedAt, DateTime.UtcNow);

            var result = await _comments.UpdateOneAsync(c => c.Id == comment.Id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _comments.DeleteOneAsync(comment => comment.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
