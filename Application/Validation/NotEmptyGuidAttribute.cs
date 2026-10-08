using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class NotEmptyGuidAttribute : ValidationAttribute
{
    public NotEmptyGuidAttribute()
    {
        ErrorMessage = "A non-empty GUID is required.";
    }

    public override bool IsValid(object? value) => value is Guid id && id != Guid.Empty;
}
