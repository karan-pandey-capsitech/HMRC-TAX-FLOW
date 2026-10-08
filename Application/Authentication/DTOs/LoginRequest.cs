namespace HMRC_TAX_FLOW.Application.Authentication.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}