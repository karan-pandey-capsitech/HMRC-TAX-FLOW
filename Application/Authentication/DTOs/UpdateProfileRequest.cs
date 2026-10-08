using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.Authentication.DTOs;

public sealed class UpdateProfileRequest
{
    [EmailAddress]
    [MinLength(3)]
    [StringLength(254)]
    public string? Email { get; init; }

    [StringLength(100)]
    public string? FullName { get; init; }
}
