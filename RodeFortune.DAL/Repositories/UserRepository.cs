using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories
{
    public class UserRepository
    {
        private readonly IMongoCollection<User> _users;

        public UserRepository(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _users.Find(_ => true).ToListAsync();
        }

        public async Task<User> GetByIdAsync(ObjectId id)
        {
            return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(User user)
        {
            await _users.InsertOneAsync(user);
        }

        public async Task UpdateAsync(ObjectId id, User updatedUser)
        {
            await _users.ReplaceOneAsync(u => u.Id == id, updatedUser);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _users.DeleteOneAsync(u => u.Id == id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
        }
    }
}
