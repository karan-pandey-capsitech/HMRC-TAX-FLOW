using HMRC_TAX_FLOW.Domain.SA100;

namespace HMRC_TAX_FLOW.Infrastructure.Repositories;

public interface ISa100Repository
{
    Task CreateAsync(Sa100Return taxReturn, CancellationToken cancellationToken = default);
    Task<Sa100Return?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sa100Return>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Sa100Return>> GetByPracticeUserAsync(
        Guid practiceUserId,
        CancellationToken cancellationToken = default);
    Task<Sa100Return?> TryUpdateDraftAsync(
        Sa100Return taxReturn,
        CancellationToken cancellationToken = default);
    Task<long> CountByStatusAsync(
        Sa100Status status,
        Guid? practiceUserId = null,
        CancellationToken cancellationToken = default);
}