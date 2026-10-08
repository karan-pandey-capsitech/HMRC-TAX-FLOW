using System.ComponentModel.DataAnnotations;
using HMRC_TAX_FLOW.Application.Validation;

namespace HMRC_TAX_FLOW.Application.Users.DTOs;

public sealed class AssignUserRoleRequest
{
    [Required]
    [AssignableUserRole]
    public string Role { get; init; } = string.Empty;
}
