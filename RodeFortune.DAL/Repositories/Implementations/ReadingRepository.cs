using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;

namespace RodeFortune.DAL.Repositories.Implementations
{
    public class ReadingRepository : IReadingRepository
    {
        private readonly IMongoCollection<Reading> _readings;

        public ReadingRepository(IMongoDatabase database)
        {
            _readings = database.GetCollection<Reading>("readings");
        }

        public async Task<List<Reading>> GetAllAsync()
        {
            return await _readings.Find(_ => true).ToListAsync();
        }

        public async Task<Reading> GetByIdAsync(ObjectId id)
        {
            return await _readings.Find(r => r.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Reading>> GetByAuthorIdAsync(ObjectId authorId)
        {
            return await _readings.Find(r => r.AuthorId == authorId).ToListAsync();
        }

        public async Task AddAsync(Reading reading)
        {
            reading.CreatedAt = DateTime.UtcNow;
            await _readings.InsertOneAsync(reading);
        }

        public async Task<bool> UpdateAsync(ObjectId id, Reading updatedReading)
        {
            var result = await _readings.ReplaceOneAsync(r => r.Id == id, updatedReading);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _readings.DeleteOneAsync(r => r.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
