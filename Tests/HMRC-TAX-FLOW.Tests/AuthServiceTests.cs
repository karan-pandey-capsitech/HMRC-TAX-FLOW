using HMRC_TAX_FLOW.Application.Authentication;
using HMRC_TAX_FLOW.Application.Authentication.DTOs;
using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Infrastructure.Authentication;
using HMRC_TAX_FLOW.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Xunit;

namespace HMRC_TAX_FLOW.Tests;

public sealed class AuthServiceTests
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    [Fact]
    public async Task RegisterAsync_TrimsFieldsHashesPasswordAndAssignsDefaultRole()
    {
        var repository = new FakeUserRepository();
        var service = CreateService(repository);
        var request = new RegisterRequest
        {
            Username = " alice ",
            Email = " alice@example.com ",
            Password = "long-enough-password",
            FullName = " Alice Example "
        };

        var user = await service.RegisterAsync(request);

        Assert.Equal("alice", user.Username);
        Assert.Equal("alice@example.com", user.Email);
        Assert.Equal("Alice Example", user.FullName);
        Assert.NotEqual(request.Password, user.PasswordHash);
        Assert.Equal(new[] { UserRole.User }, user.Roles);
        Assert.Contains(user, repository.Users);
    }

    [Fact]
    public async Task RegisterAsync_ThrowsConflictWhenUsernameAlreadyExists()
    {
        var repository = new FakeUserRepository();
        repository.Users.Add(new User { Username = "alice" });
        var service = CreateService(repository);

        await Assert.ThrowsAsync<UsernameAlreadyExistsException>(() =>
            service.RegisterAsync(new RegisterRequest
            {
                Username = "alice",
                Email = "alice@example.com",
                Password = "long-enough-password"
            }));
    }

    [Fact]
    public async Task RegisterAsync_PropagatesRepositoryDuplicateConflict()
    {
        var repository = new FakeUserRepository { ThrowDuplicateOnCreate = true };
        var service = CreateService(repository);

        await Assert.ThrowsAsync<UsernameAlreadyExistsException>(() =>
            service.RegisterAsync(new RegisterRequest
            {
                Username = "alice",
                Email = "alice@example.com",
                Password = "long-enough-password"
            }));
    }

    [Fact]
    public async Task LoginAsync_ReturnsTokenForValidPassword()
    {
        var repository = new FakeUserRepository();
        var user = new User
        {
            Username = "alice",
            Email = "alice@example.com"
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, "correct-password");
        repository.Users.Add(user);
        var service = CreateService(repository);

        var response = await service.LoginAsync(new LoginRequest
        {
            Username = " alice ",
            Password = "correct-password"
        });

        Assert.Equal("test-token", response.Token);
        Assert.Equal(new DateTime(2030, 1, 1, 0, 5, 0, DateTimeKind.Utc), response.ExpiresAt);
    }

    [Fact]
    public async Task LoginAsync_UsesSameErrorForUnknownUserAndWrongPassword()
    {
        var repository = new FakeUserRepository();
        var user = new User { Username = "alice", Email = "alice@example.com" };
        user.PasswordHash = _passwordHasher.HashPassword(user, "correct-password");
        repository.Users.Add(user);
        var service = CreateService(repository);

        var unknownUser = await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            service.LoginAsync(new LoginRequest { Username = "missing", Password = "anything" }));
        var wrongPassword = await Assert.ThrowsAsync<InvalidCredentialsException>(() =>
            service.LoginAsync(new LoginRequest { Username = "alice", Password = "wrong-password" }));

        Assert.Equal(unknownUser.Message, wrongPassword.Message);
    }

    private AuthService CreateService(FakeUserRepository repository) =>
        new(repository, new FakeJwtService(), _passwordHasher);

    private sealed class FakeJwtService : IJwtService
    {
        public TokenResult GenerateToken(User user) =>
            new()
            {
                Token = "test-token",
                ExpiresAt = new DateTime(2030, 1, 1, 0, 5, 0, DateTimeKind.Utc)
            };
    }

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];
        public bool ThrowDuplicateOnCreate { get; init; }

        public Task CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            if (ThrowDuplicateOnCreate)
            {
                throw new UsernameAlreadyExistsException();
            }

            Users.Add(user);
            return Task.CompletedTask;
        }

        public Task<User?> GetByUsernameAsync(
            string username,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.FirstOrDefault(user => user.Username == username));

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.FirstOrDefault(user => user.Id == id));

        public Task<User?> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var index = Users.FindIndex(existing => existing.Id == user.Id);
            if (index < 0)
            {
                return Task.FromResult<User?>(null);
            }

            Users[index] = user;
            return Task.FromResult<User?>(user);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.RemoveAll(user => user.Id == id) > 0);
    }
}
