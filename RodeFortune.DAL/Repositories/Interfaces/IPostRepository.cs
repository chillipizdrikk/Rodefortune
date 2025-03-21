using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface IPostRepository
    {

        public Task<List<Post>> GetAllPostsAsync();
        public Task<Post> GetByIdAsync(ObjectId id);
        public Task CreateAsync(Post post);
        public Task<bool> UpdateAsync(Post post);
        public Task<bool> DeleteAsync(ObjectId id);
    }
}
