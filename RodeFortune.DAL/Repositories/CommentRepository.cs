using MongoDB.Driver;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories
{
    public class CommentRepository
    {
        private readonly IMongoCollection<Comment> _comments;

        public CommentRepository(MongoDbContext dbContext)
        {
            _comments = dbContext.Comments;
        }

        public async Task<List<Comment>> GetAllAsync() => await _comments.Find(_ => true).ToListAsync();

        public async Task<Comment?> GetByIdAsync(string id) =>
            await _comments.Find(comment => comment.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Comment comment) => await _comments.InsertOneAsync(comment);

        public async Task UpdateAsync(string id, Comment comment) =>
            await _comments.ReplaceOneAsync(c => c.Id == id, comment);

        public async Task DeleteAsync(string id) =>
            await _comments.DeleteOneAsync(c => c.Id == id);
    }
}
