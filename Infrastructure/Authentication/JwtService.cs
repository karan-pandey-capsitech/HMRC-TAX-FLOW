using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HMRC_TAX_FLOW.Application.Abstractions.Security;
using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HMRC_TAX_FLOW.Infrastructure.Authentication;

public sealed class JwtService : IJwtService
{
    private readonly string _key;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiresMinutes;

    public JwtService(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key must be configured.");
        _issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("Jwt:Issuer must be configured.");
        _audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("Jwt:Audience must be configured.");

        if (Encoding.UTF8.GetByteCount(_key) < 32)
        {
            throw new InvalidOperationException("Jwt:Key must contain at least 32 UTF-8 bytes.");
        }

        if (!int.TryParse(configuration["Jwt:ExpiresMinutes"], out _expiresMinutes) ||
            _expiresMinutes is < 1 or > 1440)
        {
            throw new InvalidOperationException("Jwt:ExpiresMinutes must be between 1 and 1440.");
        }
    }

    public TokenResult GenerateToken(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email)
        };

        claims.AddRange(user.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt = DateTime.UtcNow.AddMinutes(_expiresMinutes);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresAt,
            signingCredentials: credentials);

        return new TokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
