using HMRC_TAX_FLOW.Application.Abstractions.Persistence;
using HMRC_TAX_FLOW.Domain.SA100;

namespace HMRC_TAX_FLOW.Application.Dashboard;

public sealed class DashboardService(ISa100Repository sa100Repository) : IDashboardService
{
    public async Task<DashboardResponse> GetAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default)
    {
        Guid? practiceUserId = isAdmin ? null : userId;
        var drafts = await sa100Repository.CountByStatusAsync(
            Sa100Status.Draft,
            practiceUserId,
            cancellationToken);
        var submitted = await sa100Repository.CountByStatusAsync(
            Sa100Status.Submitted,
            practiceUserId,
            cancellationToken);

        return new DashboardResponse(drafts + submitted, drafts, submitted);
    }
}
