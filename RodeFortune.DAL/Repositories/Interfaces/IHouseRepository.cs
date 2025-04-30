using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface IHouseRepository
    {
        public Task<List<House>> GetAllAsync();
        public Task CreateAsync(House house);

        public Task<bool> UpdateAsync(House house);
        public Task<bool> DeleteAsync(ObjectId id);
    }
}
