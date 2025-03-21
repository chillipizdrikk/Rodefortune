using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface IHoroscopeRepository
    {
        public Task<List<Horoscope>> GetAllAsync();

        public Task<Horoscope> GetByIdAsync(ObjectId id);

        public Task<List<Horoscope>> GetByZodiacSignAsync(string zodiacSign);

        public Task<List<Horoscope>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        public Task CreateAsync(Horoscope horoscope);
        public Task UpdateAsync(ObjectId id, Horoscope horoscope);

        public Task DeleteAsync(ObjectId id);
    }
}
