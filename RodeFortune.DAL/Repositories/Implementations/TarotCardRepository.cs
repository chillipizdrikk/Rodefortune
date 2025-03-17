using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using RodeFortune.DAL.Repositories.Interfaces;

namespace RodeFortune.DAL.Repositories.Implementations
{
    public class TarotCardRepository : ITarotCardRepository
    {
        private readonly IMongoCollection<TarotCard> _tarotCards;

        public TarotCardRepository(IMongoDatabase database)
        {
            _tarotCards = database.GetCollection<TarotCard>("TarotCards");
        }

        public async Task<List<TarotCard>> GetAllAsync()
        {
            return await _tarotCards.Find(_ => true).ToListAsync();
        }

       
        public async Task<TarotCard> GetCardByIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            return await _tarotCards.Find(c => c.Id == objectId).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(TarotCard card)
        {
            await _tarotCards.InsertOneAsync(card);
        }

        public async Task UpdateAsync(ObjectId id, TarotCard updatedCard)
        {
            await _tarotCards.ReplaceOneAsync(card => card.Id == id, updatedCard);
        }

        public async Task DeleteAsync(ObjectId id)
        {
            await _tarotCards.DeleteOneAsync(card => card.Id == id);
        }
    }
}
