using Microsoft.Extensions.Options;
using MongoDB.Driver;
using RodeFortune.DAL.Models;
using System.Xml.Linq;

namespace RodeFortune.DAL
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        // Оголошуємо колекції
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Post> Posts => _database.GetCollection<Post>("Posts");
        public IMongoCollection<Comment> Comments => _database.GetCollection<Comment>("Comments");
        public IMongoCollection<Reading> Readings => _database.GetCollection<Reading>("Readings");
        public IMongoCollection<Horoscope> Horoscopes => _database.GetCollection<Horoscope>("Horoscopes");
        public IMongoCollection<TarotCard> TarotCards => _database.GetCollection<TarotCard>("TarotCards");
        public IMongoCollection<NatalChart> NatalCharts => _database.GetCollection<NatalChart>("NatalCharts");
        public IMongoCollection<House> Houses => _database.GetCollection<House>("Houses");
        public IMongoCollection<DestinyMatrix> DestinyMatrices => _database.GetCollection<DestinyMatrix>("DestinyMatrices");
    }
}
