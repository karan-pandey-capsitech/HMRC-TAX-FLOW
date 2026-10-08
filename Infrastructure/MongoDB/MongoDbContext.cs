using HMRC_TAX_FLOW.Domain.Clients;
using HMRC_TAX_FLOW.Domain.SA100;
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
    public IMongoCollection<Client> Clients => _database.GetCollection<Client>("Clients");
    public IMongoCollection<Sa100Return> Sa100Returns =>
        _database.GetCollection<Sa100Return>("sa100_returns");

    public Task EnsureIndexesAsync(CancellationToken cancellationToken = default)
    {
        var usernameIndex = new CreateIndexModel<User>(
            Builders<User>.IndexKeys.Ascending(user => user.Username),
            new CreateIndexOptions
            {
                Name = "ux_users_username",
                Unique = true
            });

        var clientPracticeIndex = new CreateIndexModel<Client>(
            Builders<Client>.IndexKeys.Ascending(client => client.PracticeUserId),
            new CreateIndexOptions { Name = "ix_clients_practice_user" });
        var clientDebitamIndex = new CreateIndexModel<Client>(
            Builders<Client>.IndexKeys.Ascending(client => client.DebitamUserId),
            new CreateIndexOptions { Name = "ix_clients_debitam_user" });
        var returnClientTaxYearIndex = new CreateIndexModel<Sa100Return>(
            Builders<Sa100Return>.IndexKeys
                .Ascending(taxReturn => taxReturn.ClientId)
                .Ascending(taxReturn => taxReturn.TaxYear),
            new CreateIndexOptions
            {
                Name = "ux_sa100_client_tax_year",
                Unique = true
            });
        var returnPracticeStatusIndex = new CreateIndexModel<Sa100Return>(
            Builders<Sa100Return>.IndexKeys
                .Ascending(taxReturn => taxReturn.PracticeUserId)
                .Ascending(taxReturn => taxReturn.Status),
            new CreateIndexOptions { Name = "ix_sa100_practice_status" });

        return EnsureIndexesAsync(
            usernameIndex,
            clientPracticeIndex,
            clientDebitamIndex,
            returnClientTaxYearIndex,
            returnPracticeStatusIndex,
            cancellationToken);
    }

    private async Task EnsureIndexesAsync(
        CreateIndexModel<User> usernameIndex,
        CreateIndexModel<Client> clientPracticeIndex,
        CreateIndexModel<Client> clientDebitamIndex,
        CreateIndexModel<Sa100Return> returnClientTaxYearIndex,
        CreateIndexModel<Sa100Return> returnPracticeStatusIndex,
        CancellationToken cancellationToken)
    {
        await Users.Indexes.CreateOneAsync(usernameIndex, cancellationToken: cancellationToken);
        await Clients.Indexes.CreateManyAsync(
            [clientPracticeIndex, clientDebitamIndex],
            cancellationToken: cancellationToken);
        await Sa100Returns.Indexes.CreateManyAsync(
            [returnClientTaxYearIndex, returnPracticeStatusIndex],
            cancellationToken: cancellationToken);
    }
}