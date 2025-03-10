using MongoDB.Driver;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories
{
    public class ReadingRepository
    {
        private readonly IMongoCollection<Reading> _readings;

        public ReadingRepository(MongoDbContext dbContext)
        {
            _readings = dbContext.Readings;
        }

        public async Task<List<Reading>> GetAllAsync() => await _readings.Find(_ => true).ToListAsync();

        public async Task<Reading?> GetByIdAsync(string id) =>
            await _readings.Find(reading => reading.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Reading reading) => await _readings.InsertOneAsync(reading);

        public async Task UpdateAsync(string id, Reading reading) =>
            await _readings.ReplaceOneAsync(r => r.Id == id, reading);

        public async Task DeleteAsync(string id) =>
            await _readings.DeleteOneAsync(r => r.Id == id);
    }
}
