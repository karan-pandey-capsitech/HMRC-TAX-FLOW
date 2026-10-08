using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Infrastructure.Repositories;

namespace HMRC_TAX_FLOW.Application.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository) => _repository = repository;

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        _repository.GetByIdAsync(id, cancellationToken);

    public Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        _repository.GetByUsernameAsync(username, cancellationToken);

    public Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default) =>
        _repository.UpdateAsync(user, cancellationToken);

    public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
        _repository.DeleteAsync(id, cancellationToken);

    public async Task<User> AssignRoleAsync(
        Guid id,
        string role,
        CancellationToken cancellationToken = default)
    {
        var normalizedRole = role?.Trim();
        var allowedRole = normalizedRole is not null &&
            (normalizedRole.Equals(UserRole.User, StringComparison.OrdinalIgnoreCase) ||
             normalizedRole.Equals(UserRole.Practice, StringComparison.OrdinalIgnoreCase) ||
             normalizedRole.Equals(UserRole.Debitam, StringComparison.OrdinalIgnoreCase));

        if (!allowedRole)
        {
            throw new InvalidManagedUserRoleException();
        }

        var user = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ManagedUserNotFoundException();

        var canonicalRole = normalizedRole!.Equals(UserRole.User, StringComparison.OrdinalIgnoreCase)
            ? UserRole.User
            : normalizedRole.Equals(UserRole.Practice, StringComparison.OrdinalIgnoreCase)
                ? UserRole.Practice
                : UserRole.Debitam;

        user.Roles = [canonicalRole];
        return await _repository.UpdateAsync(user, cancellationToken)
            ?? throw new ManagedUserNotFoundException();
    }
}
