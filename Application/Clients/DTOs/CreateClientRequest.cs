using System.ComponentModel.DataAnnotations;

namespace HMRC_TAX_FLOW.Application.Clients.DTOs;

public sealed class CreateClientRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string NationalInsuranceNumber { get; init; } = string.Empty;

    [Required]
    public Guid PracticeUserId { get; init; }

    public Guid? DebitamUserId { get; init; }
}
