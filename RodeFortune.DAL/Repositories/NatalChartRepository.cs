using MongoDB.Driver;
using RodeFortune.DAL.Models;

namespace RodeFortune.DAL.Repositories
{
    public class NatalChartRepository
    {
        private readonly IMongoCollection<NatalChart> _natalCharts;

        public NatalChartRepository(MongoDbContext dbContext)
        {
            _natalCharts = dbContext.NatalCharts;
        }

        public async Task<List<NatalChart>> GetAllAsync() => await _natalCharts.Find(_ => true).ToListAsync();

        public async Task<NatalChart?> GetByIdAsync(string id) =>
            await _natalCharts.Find(natalChart => natalChart.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(NatalChart natalChart) => await _natalCharts.InsertOneAsync(natalChart);

        public async Task UpdateAsync(string id, NatalChart natalChart) =>
            await _natalCharts.ReplaceOneAsync(n => n.Id == id, natalChart);

        public async Task DeleteAsync(string id) =>
            await _natalCharts.DeleteOneAsync(n => n.Id == id);
    }
}
