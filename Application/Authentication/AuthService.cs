using System;
using System.Threading.Tasks;
using HMRC_TAX_FLOW.Application.Authentication.DTOs;
using HMRC_TAX_FLOW.Domain.Users;
using HMRC_TAX_FLOW.Infrastructure.Repositories;
using HMRC_TAX_FLOW.Infrastructure.Authentication;
using Microsoft.AspNetCore.Identity;

namespace HMRC_TAX_FLOW.Application.Authentication
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly IJwtService _jwt;
        private readonly IPasswordHasher<User> _hasher;

        public AuthService(IUserRepository repo, IJwtService jwt, IPasswordHasher<User> hasher)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
            _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var user = await _repo.GetByUsernameAsync(request.Username);
            if (user == null) throw new InvalidOperationException("Invalid credentials.");

            var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (verify == PasswordVerificationResult.Failed) throw new InvalidOperationException("Invalid credentials.");

            var token = _jwt.GenerateToken(user);
            return new LoginResponse { Token = token.Token, ExpiresAt = token.ExpiresAt };
        }

        public async Task<User> RegisterAsync(RegisterRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            var existing = await _repo.GetByUsernameAsync(request.Username);
            if (existing != null) throw new InvalidOperationException("Username already exists.");

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                FullName = request.FullName
            };

            user.PasswordHash = _hasher.HashPassword(user, request.Password);
            if (!string.IsNullOrWhiteSpace(request.Role)) user.Roles = new System.Collections.Generic.List<string> { request.Role! };

            await _repo.CreateAsync(user);
            return user;
        }
    }
}