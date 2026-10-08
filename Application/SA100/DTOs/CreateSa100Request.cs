using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.SA100.DTOs;

public sealed class CreateSa100Request
{
    [Required]
    public Guid ClientId { get; init; }

    [Required]
    [RegularExpression(@"^\d{4}-\d{2}$", ErrorMessage = "Tax year must use the YYYY-YY format.")]
    public string TaxYear { get; init; } = string.Empty;

    [Range(typeof(decimal), "0", "999999999.99")]
    public decimal EmploymentIncome { get; init; }

    [Range(typeof(decimal), "0", "999999999.99")]
    public decimal SelfEmploymentIncome { get; init; }

    [Range(typeof(decimal), "0", "999999999.99")]
    public decimal OtherIncome { get; init; }

    [Range(typeof(decimal), "0", "999999999.99")]
    public decimal TaxAlreadyPaid { get; init; }

    [Range(typeof(decimal), "0", "999999999.99")]
    public decimal EstimatedTax { get; init; }
}
