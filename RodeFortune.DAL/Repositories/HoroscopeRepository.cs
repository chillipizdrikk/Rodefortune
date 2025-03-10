using MongoDB.Driver;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories
{
    public class HoroscopeRepository
    {
        private readonly IMongoCollection<Horoscope> _horoscopes;

        public HoroscopeRepository(MongoDbContext dbContext)
        {
            _horoscopes = dbContext.Horoscopes;
        }

        public async Task<List<Horoscope>> GetAllAsync() => await _horoscopes.Find(_ => true).ToListAsync();

        public async Task<Horoscope?> GetByIdAsync(string id) =>
            await _horoscopes.Find(horoscope => horoscope.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Horoscope horoscope) => await _horoscopes.InsertOneAsync(horoscope);

        public async Task UpdateAsync(string id, Horoscope horoscope) =>
            await _horoscopes.ReplaceOneAsync(h => h.Id == id, horoscope);

        public async Task DeleteAsync(string id) =>
            await _horoscopes.DeleteOneAsync(h => h.Id == id);
    }
}
