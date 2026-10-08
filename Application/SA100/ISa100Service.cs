using HMRC_TAX_FLOW.Application.SA100.DTOs;

namespace HMRC_TAX_FLOW.Application.SA100;

public interface ISa100Service
{
    Task<IReadOnlyList<Sa100Response>> GetAllAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<Sa100Response> GetByIdAsync(
        Guid id,
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);

    Task<Sa100Response> CreateAsync(
        CreateSa100Request request,
        Guid practiceUserId,
        CancellationToken cancellationToken = default);

    Task<Sa100Response> UpdateDraftAsync(
        Guid id,
        UpdateSa100Request request,
        Guid practiceUserId,
        CancellationToken cancellationToken = default);

    Task<Sa100Response> SubmitAsync(
        Guid id,
        Guid practiceUserId,
        CancellationToken cancellationToken = default);
}
