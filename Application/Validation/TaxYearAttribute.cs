using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text.RegularExpressions;

namespace HMRC_TAX_FLOW.Application.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class TaxYearAttribute : ValidationAttribute
{
    private static readonly Regex Format = new("^(?<start>\\d{4})-(?<end>\\d{2})$", RegexOptions.Compiled);

    public TaxYearAttribute()
    {
        ErrorMessage = "Tax year must be a consecutive YYYY-YY range, for example 2025-26.";
    }

    public override bool IsValid(object? value)
    {
        if (value is not string taxYear)
        {
            return false;
        }

        var match = Format.Match(taxYear);
        if (!match.Success ||
            !int.TryParse(match.Groups["start"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var startYear) ||
            !int.TryParse(match.Groups["end"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var endYear))
        {
            return false;
        }

        return startYear is >= 1900 and < 9999 && endYear == (startYear + 1) % 100;
    }
}
