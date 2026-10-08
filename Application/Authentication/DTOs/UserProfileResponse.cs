namespace HMRC_TAX_FLOW.Application.Authentication.DTOs;

public sealed record UserProfileResponse(
    Guid Id,
    string Username,
    string Email,
    string? FullName,
    IReadOnlyList<string> Roles);
