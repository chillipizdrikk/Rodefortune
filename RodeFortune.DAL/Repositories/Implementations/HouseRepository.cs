using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;

namespace RodeFortune.DAL.Repositories.Implementations
{
    public class HouseRepository : IHouseRepository
    {
        private readonly IMongoCollection<House> _houses;

        public HouseRepository(IMongoDatabase database)
        {
            _houses = database.GetCollection<House>("houses");
        }

        public async Task<List<House>> GetAllAsync()
        {
            return await _houses.Find(_ => true).ToListAsync();
        }

        public async Task<House> GetByIdAsync(ObjectId id)
        {
            return await _houses.Find(h => h.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(House house)
        {
            await _houses.InsertOneAsync(house);
        }

        public async Task<bool> UpdateAsync(House house)
        {
            var result = await _houses.ReplaceOneAsync(h => h.Id == house.Id, house);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _houses.DeleteOneAsync(h => h.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
