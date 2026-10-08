using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.Authentication.DTOs;

public sealed class LoginRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; init; } = string.Empty;

    [Required]
    [StringLength(128)]
    public string Password { get; init; } = string.Empty;
}