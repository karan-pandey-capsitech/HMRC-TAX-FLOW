using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Application.Users;

public interface IUserService
{
    Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<User> AssignRoleAsync(Guid id, string role, CancellationToken cancellationToken = default);
}
