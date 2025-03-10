using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories
{
    public class HoroscopeRepository
    {
        private readonly IMongoCollection<Horoscope> _horoscopeCollection;

        public HoroscopeRepository(IMongoDatabase database)
        {
            _horoscopeCollection = database.GetCollection<Horoscope>("horoscopes");
        }

        public async Task<List<Horoscope>> GetAllAsync()
        {
            return await _horoscopeCollection.Find(_ => true).ToListAsync();
        }

        public async Task<Horoscope> GetByIdAsync(ObjectId id)
        {
            return await _horoscopeCollection.Find(h => h.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Horoscope>> GetByZodiacSignAsync(string zodiacSign)
        {
            return await _horoscopeCollection.Find(h => h.ZodiacSign == zodiacSign).ToListAsync();
        }

        public async Task CreateAsync(Horoscope horoscope)
        {
            await _horoscopeCollection.InsertOneAsync(horoscope);
        }

        public async Task UpdateAsync(ObjectId id, Horoscope updatedHoroscope)
        {
            await _horoscopeCollection.ReplaceOneAsync(h => h.Id == id, updatedHoroscope);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _horoscopeCollection.DeleteOneAsync(h => h.Id == id);
        }
    }
}
