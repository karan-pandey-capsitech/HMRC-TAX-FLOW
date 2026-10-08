using HMRC_TAX_FLOW.Domain.SA100;

namespace HMRC_TAX_FLOW.Application.SA100.DTOs;

public sealed record Sa100Response(
    Guid Id,
    Guid ClientId,
    string TaxYear,
    string ClientName,
    string NationalInsuranceNumber,
    decimal EmploymentIncome,
    decimal SelfEmploymentIncome,
    decimal OtherIncome,
    decimal TaxAlreadyPaid,
    decimal EstimatedTax,
    Sa100Status Status,
    Guid CreatedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    Guid? SubmittedBy,
    DateTime? SubmittedAt);
