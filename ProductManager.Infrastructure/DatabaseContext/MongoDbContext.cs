using ProductManager.Core.Domain.Entities;
using MongoDB.Driver;

namespace ProductManager.Infrastructure.DatabaseContext
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(string connectionString, string databaseName)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException(nameof(connectionString));
            if (string.IsNullOrEmpty(databaseName))
                throw new ArgumentNullException(nameof(databaseName));

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public virtual IMongoCollection<Product> Products => _database.GetCollection<Product>("Products");

        public void EnsureIndexes()
        {
            var indexKeys = Builders<Product>.IndexKeys.Ascending(p => p.Name);
            Products.Indexes.CreateOne(new CreateIndexModel<Product>(indexKeys));

            var emailIndex = Builders<Product>.IndexKeys.Ascending(p => p.ManufactureEmail);
            Products.Indexes.CreateOne(new CreateIndexModel<Product>(emailIndex));
        }
    }
}
