using HMRC_TAX_FLOW.Domain.Clients;

namespace HMRC_TAX_FLOW.Application.Abstractions.Persistence;

public interface IClientRepository
{
    Task CreateAsync(Client client, CancellationToken cancellationToken = default);
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Client>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Client>> GetByAssignedUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
