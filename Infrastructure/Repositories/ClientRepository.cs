using HMRC_TAX_FLOW.Domain.Clients;
using HMRC_TAX_FLOW.Application.Abstractions.Persistence;
using HMRC_TAX_FLOW.Infrastructure.MongoDB;
using MongoDB.Driver;

namespace HMRC_TAX_FLOW.Infrastructure.Repositories;

public sealed class ClientRepository(MongoDbContext context) : IClientRepository
{
    public Task CreateAsync(Client client, CancellationToken cancellationToken = default) =>
        context.Clients.InsertOneAsync(client, cancellationToken: cancellationToken);

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await context.Clients.Find(client => client.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await context.Clients.Find(Builders<Client>.Filter.Empty)
            .SortBy(client => client.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Client>> GetByAssignedUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await context.Clients.Find(
                client => client.PracticeUserId == userId || client.DebitamUserId == userId)
            .SortBy(client => client.Name)
            .ToListAsync(cancellationToken);
}
