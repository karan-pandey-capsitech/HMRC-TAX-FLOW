using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HMRC_TAX_FLOW.Infrastructure.MongoDB;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);
    }

    public IMongoCollection<User> Users => _database.GetCollection<User>("Users");

    public Task EnsureIndexesAsync(CancellationToken cancellationToken = default)
    {
        var usernameIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(user => user.Username),
            new CreateIndexOptions
            {
                Name = "ux_users_username",
                Unique = true
            });

        return Users.Indexes.CreateOneAsync(usernameIndex, cancellationToken: cancellationToken);
    }
}