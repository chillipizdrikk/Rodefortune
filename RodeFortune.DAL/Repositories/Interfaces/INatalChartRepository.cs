using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface INatalChartRepository
    {
        public Task<List<NatalChart>> GetAllAsync();

        public Task<NatalChart> GetByIdAsync(ObjectId id);
        public Task<List<NatalChart>> GetByUserIdAsync(ObjectId userId);
        public Task AddAsync(NatalChart natalChart);

        public Task<bool> UpdateAsync(ObjectId id, NatalChart updatedNatalChart);

        public Task<bool> DeleteAsync(ObjectId id);
    }
}
