using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class MoneyAmountAttribute : RangeAttribute
{
    public MoneyAmountAttribute() : base(typeof(decimal), "0", "999999999.99")
    {
        ErrorMessage = "Amount must be between 0 and 999999999.99.";
    }
}
