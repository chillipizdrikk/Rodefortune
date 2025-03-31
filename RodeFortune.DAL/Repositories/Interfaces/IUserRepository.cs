using MongoDB.Bson;
using RodeFortune.DAL.Models;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(ObjectId id);
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task CreateAsync(User user);
        Task<bool> UpdateAsync(ObjectId id, User user);
        Task<bool> DeleteAsync(ObjectId id);
    }
}