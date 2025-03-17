using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories.Implementations
{
    public class PostRepository
    {
        private readonly IMongoCollection<Post> _posts;

        public PostRepository(IMongoDatabase database)
        {
            _posts = database.GetCollection<Post>("posts");
        }

        public async Task<List<Post>> GetAllAsync()
        {
            return await _posts.Find(_ => true).ToListAsync();
        }

        public async Task<Post> GetByIdAsync(ObjectId id)
        {
            return await _posts.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(Post post)
        {
            await _posts.InsertOneAsync(post);
        }

        public async Task<bool> UpdateAsync(Post post)
        {
            var result = await _posts.ReplaceOneAsync(p => p.Id == post.Id, post);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _posts.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
