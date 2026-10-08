using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.Users.DTOs;

public sealed class AssignUserRoleRequest
{
    [Required]
    [RegularExpression("^(User|Practice|Debitam)$", ErrorMessage = "Role must be User, Practice, or Debitam.")]
    public string Role { get; init; } = string.Empty;
}
