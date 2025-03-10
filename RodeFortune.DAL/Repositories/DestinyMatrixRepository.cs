using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories
{
    public class DestinyMatrixRepository
    {
        private readonly IMongoCollection<DestinyMatrix> _collection;

        public DestinyMatrixRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<DestinyMatrix>("destiny_matrices");
        }

        public async Task<List<DestinyMatrix>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task<DestinyMatrix> GetByIdAsync(ObjectId id)
        {
            return await _collection.Find(dm => dm.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<DestinyMatrix>> GetByUserIdAsync(ObjectId userId)
        {
            return await _collection.Find(dm => dm.UserId == userId).ToListAsync();
        }

        public async Task CreateAsync(DestinyMatrix destinyMatrix)
        {
            await _collection.InsertOneAsync(destinyMatrix);
        }

        public async Task<bool> UpdateContentAsync(ObjectId id, string content)
        {
            var update = Builders<DestinyMatrix>.Update.Set(dm => dm.Content, content);
            var result = await _collection.UpdateOneAsync(dm => dm.Id == id, update);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _collection.DeleteOneAsync(dm => dm.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
