namespace HMRC_TAX_FLOW.Application.Dashboard;

public sealed record DashboardResponse(
    long TotalReturns,
    long DraftReturns,
    long SubmittedReturns);
