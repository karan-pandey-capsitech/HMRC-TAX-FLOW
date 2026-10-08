using MongoDB.Driver;
using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.Extensions.Options;

namespace HMRC_TAX_FLOW.Infrastructure.MongoDB
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
    }
}