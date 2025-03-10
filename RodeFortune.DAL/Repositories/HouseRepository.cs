using MongoDB.Driver;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories
{
    public class HouseRepository
    {
        private readonly IMongoCollection<House> _houses;

        public HouseRepository(MongoDbContext dbContext)
        {
            _houses = dbContext.Houses;
        }

        public async Task<List<House>> GetAllAsync() => await _houses.Find(_ => true).ToListAsync();

        public async Task<House?> GetByIdAsync(string id) =>
            await _houses.Find(house => house.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(House house) => await _houses.InsertOneAsync(house);

        public async Task UpdateAsync(string id, House house) =>
            await _houses.ReplaceOneAsync(h => h.Id == id, house);

        public async Task DeleteAsync(string id) =>
            await _houses.DeleteOneAsync(h => h.Id == id);
    }
}
