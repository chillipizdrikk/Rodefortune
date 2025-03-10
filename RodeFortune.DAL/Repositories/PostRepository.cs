using MongoDB.Driver;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories
{
    public class PostRepository
    {
        private readonly IMongoCollection<Post> _posts;

        public PostRepository(MongoDbContext dbContext)
        {
            _posts = dbContext.Posts;
        }

        public async Task<List<Post>> GetAllAsync() => await _posts.Find(_ => true).ToListAsync();

        public async Task<Post?> GetByIdAsync(string id) =>
            await _posts.Find(post => post.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Post post) => await _posts.InsertOneAsync(post);

        public async Task UpdateAsync(string id, Post post) =>
            await _posts.ReplaceOneAsync(p => p.Id == id, post);

        public async Task DeleteAsync(string id) =>
            await _posts.DeleteOneAsync(p => p.Id == id);
    }
}
