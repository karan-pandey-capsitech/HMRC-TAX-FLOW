using System;

namespace HMRC_TAX_FLOW.Application.Authentication.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
    }
}