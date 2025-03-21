using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;

namespace RodeFortune.DAL.Repositories.Implementations
{
    public class HoroscopeRepository : IHoroscopeRepository
    {
        private readonly IMongoCollection<Horoscope> _collection;

        public HoroscopeRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Horoscope>("Horoscopes");
        }

        public async Task<List<Horoscope>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<Horoscope> GetByIdAsync(ObjectId id)
        {
            var filter = Builders<Horoscope>.Filter.Eq(h => h.Id, id);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<Horoscope>> GetByZodiacSignAsync(string zodiacSign)
        {
            var filter = Builders<Horoscope>.Filter.Eq(h => h.ZodiacSign, zodiacSign);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<List<Horoscope>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Horoscope>.Filter.And(
                Builders<Horoscope>.Filter.Gte(h => h.Date, startDate),
                Builders<Horoscope>.Filter.Lte(h => h.Date, endDate)
            );

            return await _collection.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(Horoscope horoscope)
        {
            await _collection.InsertOneAsync(horoscope);
        }

        public async Task UpdateAsync(ObjectId id, Horoscope horoscope)
        {
            var filter = Builders<Horoscope>.Filter.Eq(h => h.Id, id);
            await _collection.ReplaceOneAsync(filter, horoscope);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            var filter = Builders<Horoscope>.Filter.Eq(h => h.Id, id);
            await _collection.DeleteOneAsync(filter);
        }
    }
}