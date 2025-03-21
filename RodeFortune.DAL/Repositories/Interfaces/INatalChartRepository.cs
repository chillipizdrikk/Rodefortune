using MongoDB.Bson;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RodeFortune.DAL.Repositories.Interfaces
{
    public interface INatalChartRepository
    {
        public Task<List<NatalChart>> GetAllAsync();

        public Task<NatalChart> GetByIdAsync(ObjectId id);
        public Task<List<NatalChart>> GetByUserIdAsync(ObjectId userId);
        public Task AddAsync(NatalChart natalChart);

        public Task<bool> UpdateAsync(ObjectId id, NatalChart updatedNatalChart);

        public Task<bool> DeleteAsync(ObjectId id);
    }
}
