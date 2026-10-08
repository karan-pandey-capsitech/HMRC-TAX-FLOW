namespace HMRC_TAX_FLOW.Domain.Clients;

public sealed class Client
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string NationalInsuranceNumber { get; set; } = string.Empty;
    public Guid PracticeUserId { get; set; }
    public Guid? DebitamUserId { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}