using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface IReadingRepository
    {
        public Task<List<Reading>> GetAllAsync();
        public Task<Reading> GetByIdAsync(ObjectId id);
        public Task<List<Reading>> GetByAuthorIdAsync(ObjectId authorId);
        public Task AddAsync(Reading reading);
        public Task<bool> UpdateAsync(ObjectId id, Reading updatedReading);
        public Task<bool> DeleteAsync(ObjectId id);
    }
}
