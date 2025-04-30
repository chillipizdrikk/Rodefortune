using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface IDestinyMatrixRepository
    {

        public Task<List<DestinyMatrix>> GetAllAsync();

        public Task<DestinyMatrix> GetByIdAsync(ObjectId id);
        public Task<List<DestinyMatrix>> GetByUserIdAsync(ObjectId userId);
        public Task CreateAsync(DestinyMatrix destinyMatrix);
        public Task<bool> UpdateContentAsync(ObjectId id, string content);
        public Task<bool> DeleteAsync(ObjectId id);
    }
}
