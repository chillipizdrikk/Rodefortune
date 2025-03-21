using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {

        public Task<List<User>> GetAllAsync();
        public Task<User> GetByIdAsync(ObjectId id);
        public Task CreateAsync(User user);
        public Task UpdateAsync(ObjectId id, User updatedUser);
        public Task DeleteAsync(ObjectId id);
        public Task<User> GetByEmailAsync(string email);
    }
}
