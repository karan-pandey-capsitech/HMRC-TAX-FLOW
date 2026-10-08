using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.SA100.DTOs;

public sealed class UpdateSa100Request
{
    [Required]
    [StringLength(100)]
    public string ClientName { get; init; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string NationalInsuranceNumber { get; init; } = string.Empty;

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
