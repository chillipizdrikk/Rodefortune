using MongoDB.Bson;
using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface ITarotCardRepository
    {
        public  Task<List<TarotCard>> GetAllAsync();
        public  Task<TarotCard> GetCardByIdAsync(string id);

        public Task CreateAsync(TarotCard card);

        public Task UpdateAsync(ObjectId id, TarotCard updatedCard);

        public Task DeleteAsync(ObjectId id);
    }
}
