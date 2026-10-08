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
}