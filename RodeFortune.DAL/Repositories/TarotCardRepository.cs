using MongoDB.Driver;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories
{
    public class TarotCardRepository
    {
        private readonly IMongoCollection<TarotCard> _tarotCards;

        public TarotCardRepository(MongoDbContext dbContext)
        {
            _tarotCards = dbContext.TarotCards;
        }

        public async Task<List<TarotCard>> GetAllAsync() => await _tarotCards.Find(_ => true).ToListAsync();

        public async Task<TarotCard?> GetByIdAsync(string id) =>
            await _tarotCards.Find(tarotCard => tarotCard.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(TarotCard tarotCard) => await _tarotCards.InsertOneAsync(tarotCard);

        public async Task UpdateAsync(string id, TarotCard tarotCard) =>
            await _tarotCards.ReplaceOneAsync(t => t.Id == id, tarotCard);

        public async Task DeleteAsync(string id) =>
            await _tarotCards.DeleteOneAsync(t => t.Id == id);
    }
}
