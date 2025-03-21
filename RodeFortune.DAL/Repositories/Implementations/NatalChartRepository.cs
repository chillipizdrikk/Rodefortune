using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using RodeFortune.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories.Implementations
{
    public class NatalChartRepository : INatalChartRepository
    {
        private readonly IMongoCollection<NatalChart> _natalCharts;

        public NatalChartRepository(IMongoDatabase database)
        {
            _natalCharts = database.GetCollection<NatalChart>("NatalCharts");
        }

        public async Task<List<NatalChart>> GetAllAsync()
        {
            return await _natalCharts.Find(_ => true).ToListAsync();
        }

        public async Task<NatalChart> GetByIdAsync(ObjectId id)
        {
            return await _natalCharts.Find(chart => chart.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<NatalChart>> GetByUserIdAsync(ObjectId userId)
        {
            return await _natalCharts.Find(chart => chart.UserId == userId).ToListAsync();
        }

        public async Task AddAsync(NatalChart natalChart)
        {
            natalChart.CreatedAt = DateTime.UtcNow;
            await _natalCharts.InsertOneAsync(natalChart);
        }

        public async Task<bool> UpdateAsync(ObjectId id, NatalChart updatedNatalChart)
        {
            var result = await _natalCharts.ReplaceOneAsync(chart => chart.Id == id, updatedNatalChart);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync(ObjectId id)
        {
            var result = await _natalCharts.DeleteOneAsync(chart => chart.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
