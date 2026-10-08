using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Application.Abstractions.Security;

public interface IJwtService
{
    TokenResult GenerateToken(User user);
}

public sealed record TokenResult(string Token, DateTime ExpiresAt);
