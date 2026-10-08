using HMRC_TAX_FLOW.Application.Authentication.DTOs;
using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Infrastructure.Authentication;
using HMRC_TAX_FLOW.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;

namespace HMRC_TAX_FLOW.Application.Authentication;

public sealed class AuthService : IAuthService
{
    private readonly IUserRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository repository,
        IJwtService jwtService,
        IPasswordHasher<User> passwordHasher)
    {
        _repository = repository;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var username = request.Username.Trim();
        var user = await _repository.GetByUsernameAsync(username, cancellationToken);
        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        var verificationResult = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            request.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            throw new InvalidCredentialsException();
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
            if (await _repository.UpdateAsync(user, cancellationToken) is null)
            {
                throw new InvalidCredentialsException();
            }
        }

        var token = _jwtService.GenerateToken(user);
        return new LoginResponse
        {
            Token = token.Token,
            ExpiresAt = token.ExpiresAt
        };
    }

    public async Task<User> RegisterAsync(
        RegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var username = request.Username.Trim();
        if (await _repository.GetByUsernameAsync(username, cancellationToken) is not null)
        {
            throw new UsernameAlreadyExistsException();
        }

        var user = new User
        {
            Username = username,
            Email = request.Email.Trim(),
            FullName = string.IsNullOrWhiteSpace(request.FullName)
                ? null
                : request.FullName.Trim()
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        // Registration is public, so the caller must not be allowed to choose a role.
        await _repository.CreateAsync(user, cancellationToken);
        return user;
    }
}