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
    public interface IDestinyMatrixRepository
    {

        public Task<List<DestinyMatrix>> GetAllAsync();

        public Task<DestinyMatrix> GetByIdAsync(ObjectId id);
        public Task<List<DestinyMatrix>> GetByUserIdAsync(ObjectId userId);
        public Task CreateAsync(DestinyMatrix destinyMatrix);
        public Task<bool> UpdateContentAsync(ObjectId id, string content);
        public Task<bool> DeleteAsync(ObjectId id);
    }
}
