namespace HMRC_TAX_FLOW.Application.Dashboard;

public interface IDashboardService
{
    Task<DashboardResponse> GetAsync(
        Guid userId,
        bool isAdmin,
        CancellationToken cancellationToken = default);
}
