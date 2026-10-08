using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Application.Abstractions.Persistence;

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
        if (!UserRole.TryNormalizeAssignable(role, out var normalizedRole))
        {
            throw new InvalidManagedUserRoleException();
        }

        var user = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new ManagedUserNotFoundException();

        user.Roles = [normalizedRole];
        return await _repository.UpdateAsync(user, cancellationToken)
            ?? throw new ManagedUserNotFoundException();
    }
}
