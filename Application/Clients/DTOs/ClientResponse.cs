namespace HMRC_TAX_FLOW.Application.Clients.DTOs;

public sealed record ClientResponse(
    Guid Id,
    string Name,
    string NationalInsuranceNumber,
    Guid PracticeUserId,
    Guid? DebitamUserId,
    DateTime CreatedAt);
