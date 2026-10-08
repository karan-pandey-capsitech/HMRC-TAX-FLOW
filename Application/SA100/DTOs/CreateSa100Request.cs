using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using HMRC_TAX_FLOW.Application.Validation;

namespace HMRC_TAX_FLOW.Application.SA100.DTOs;

public sealed class CreateSa100Request
{
    [JsonRequired]
    [NotEmptyGuid]
    public Guid ClientId { get; init; }

    [Required]
    [JsonRequired]
    [TaxYear]
    public string TaxYear { get; init; } = string.Empty;

    [JsonRequired]
    [MoneyAmount]
    public decimal EmploymentIncome { get; init; }

    [JsonRequired]
    [MoneyAmount]
    public decimal SelfEmploymentIncome { get; init; }

    [JsonRequired]
    [MoneyAmount]
    public decimal OtherIncome { get; init; }

    [JsonRequired]
    [MoneyAmount]
    public decimal TaxAlreadyPaid { get; init; }

    [JsonRequired]
    [MoneyAmount]
    public decimal EstimatedTax { get; init; }
}
