using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<bool> TryCreateAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
