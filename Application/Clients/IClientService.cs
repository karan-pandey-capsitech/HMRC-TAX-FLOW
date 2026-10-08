using HMRC_TAX_FLOW.Application.Clients.DTOs;

namespace HMRC_TAX_FLOW.Application.Clients;

public interface IClientService
{
    Task<IReadOnlyList<ClientResponse>> GetAllAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<ClientResponse> CreateAsync(
        CreateClientRequest request,
        Guid createdBy,
        CancellationToken cancellationToken = default);
}
