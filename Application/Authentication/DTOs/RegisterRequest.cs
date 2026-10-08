namespace HMRC_TAX_FLOW.Application.Authentication.DTOs
{
    public class RegisterRequest
    {
        public string Username { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? FullName { get; set; }
        public string? Role { get; set; } // optional
    }
}