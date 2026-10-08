using System.ComponentModel.DataAnnotations;
using HMRC_TAX_FLOW.Domain.Users;

namespace HMRC_TAX_FLOW.Application.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class AssignableUserRoleAttribute : ValidationAttribute
{
    public AssignableUserRoleAttribute()
    {
        ErrorMessage = "Role must be User, Practice, or Debitam.";
    }

    public override bool IsValid(object? value) =>
        value is string role && UserRole.TryNormalizeAssignable(role, out _);
}
