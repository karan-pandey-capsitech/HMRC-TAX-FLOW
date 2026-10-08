namespace HMRC_TAX_FLOW.Domain.SA100;

public sealed class Sa100Return
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ClientId { get; set; }
    public Guid PracticeUserId { get; set; }
    public string TaxYear { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string NationalInsuranceNumber { get; set; } = string.Empty;
    public decimal EmploymentIncome { get; set; }
    public decimal SelfEmploymentIncome { get; set; }
    public decimal OtherIncome { get; set; }
    public decimal TaxAlreadyPaid { get; set; }
    public decimal EstimatedTax { get; set; }
    public Sa100Status Status { get; set; } = Sa100Status.Draft;
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid? SubmittedBy { get; set; }
    public DateTime? SubmittedAt { get; set; }
}