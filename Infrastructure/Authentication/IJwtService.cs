using System;
using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Infrastructure.Authentication
{
    public class TokenResult
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
    }

    public interface IJwtService
    {
        TokenResult GenerateToken(User user);
    }
}