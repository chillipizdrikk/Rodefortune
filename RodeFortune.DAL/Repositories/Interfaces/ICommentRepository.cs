using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface ICommentRepository
    {
        public Task<List<Comment>> GetAllAsync();
        public Task<Comment> GetByIdAsync(ObjectId id);

        public Task<List<Comment>> GetByPostIdAsync(ObjectId postId);
        public Task CreateAsync(Comment comment);
        public Task<bool> UpdateAsync(Comment comment);
        public Task<bool> DeleteAsync(ObjectId id);
    }
}
