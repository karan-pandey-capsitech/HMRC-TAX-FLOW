using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.Authentication.DTOs;

public sealed class RegisterRequest
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    [RegularExpression(@"^[A-Za-z0-9._-]+$", ErrorMessage = "Username may contain only letters, numbers, periods, underscores, and hyphens.")]
    public string Username { get; init; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(254)]
    public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 12)]
    public string Password { get; init; } = string.Empty;

    [StringLength(100)]
    public string? FullName { get; init; }
}