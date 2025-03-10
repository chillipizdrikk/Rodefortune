using MongoDB.Driver;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories
{
    public class DestinyMatrixRepository
    {
        private readonly IMongoCollection<DestinyMatrix> _destinyMatrices;

        public DestinyMatrixRepository(MongoDbContext dbContext)
        {
            _destinyMatrices = dbContext.DestinyMatrices;
        }

        public async Task<List<DestinyMatrix>> GetAllAsync() => await _destinyMatrices.Find(_ => true).ToListAsync();

        public async Task<DestinyMatrix?> GetByIdAsync(string id) =>
            await _destinyMatrices.Find(destinyMatrix => destinyMatrix.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(DestinyMatrix destinyMatrix) => await _destinyMatrices.InsertOneAsync(destinyMatrix);

        public async Task UpdateAsync(string id, DestinyMatrix destinyMatrix) =>
            await _destinyMatrices.ReplaceOneAsync(d => d.Id == id, destinyMatrix);

        public async Task DeleteAsync(string id) =>
            await _destinyMatrices.DeleteOneAsync(d => d.Id == id);
    }
}
