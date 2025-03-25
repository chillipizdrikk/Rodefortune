using MongoDB.Bson;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface ITarotCardRepository
    {
        public Task<List<TarotCard>> GetAllAsync();
        public Task<TarotCard> GetCardByIdAsync(string id);

        public Task CreateAsync(TarotCard card);

        public Task UpdateAsync(ObjectId id, TarotCard updatedCard);

        public Task DeleteAsync(ObjectId id);

        public Task DeleteAsync(string id);

        public Task<TarotCard?> GetCardByNameAsync(string name);
        public Task DeleteByNameAsync(string name);
    }
}
