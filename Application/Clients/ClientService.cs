using HMRC_TAX_FLOW.Application.Clients.DTOs;
using HMRC_TAX_FLOW.Application.Abstractions.Persistence;
using HMRC_TAX_FLOW.Domain.Clients;
using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Application.Clients;

public sealed class ClientService(
    IClientRepository clientRepository,
    IUserRepository userRepository) : IClientService
{
    public async Task<IReadOnlyList<ClientResponse>> GetAllAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        var clients = isAdmin
            ? await clientRepository.GetAllAsync(cancellationToken)
            : await clientRepository.GetByAssignedUserAsync(userId, cancellationToken);
        return clients.Select(ToResponse).ToArray();
    }

    public async Task<ClientResponse> CreateAsync(
        CreateClientRequest request,
        Guid createdBy,
        CancellationToken cancellationToken = default)
    {
        var practiceUser = await userRepository.GetByIdAsync(request.PracticeUserId, cancellationToken);
        if (practiceUser is null || !practiceUser.Roles.Contains(UserRole.Practice))
        {
            throw new InvalidClientAssignmentException("The assigned user must have the Practice role.");
        }

        if (request.DebitamUserId is Guid debitamUserId)
        {
            var debitamUser = await userRepository.GetByIdAsync(debitamUserId, cancellationToken);
            if (debitamUser is null || !debitamUser.Roles.Contains(UserRole.Debitam))
            {
                throw new InvalidClientAssignmentException("The assigned user must have the Debitam role.");
            }
        }

        var client = new Client
        {
            Name = request.Name.Trim(),
            NationalInsuranceNumber = request.NationalInsuranceNumber.Trim().ToUpperInvariant(),
            PracticeUserId = request.PracticeUserId,
            DebitamUserId = request.DebitamUserId,
            CreatedBy = createdBy
        };

        await clientRepository.CreateAsync(client, cancellationToken);
        return ToResponse(client);
    }

    private static ClientResponse ToResponse(Client client) =>
        new(
            client.Id,
            client.Name,
            client.NationalInsuranceNumber,
            client.PracticeUserId,
            client.DebitamUserId,
            client.CreatedAt);
}
