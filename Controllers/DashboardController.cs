using System.Security.Claims;
using HMRC_TAX_FLOW.Application.Dashboard;
using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMRC_TAX_FLOW.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = $"{UserRole.Admin},{UserRole.Practice}")]
public sealed class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardResponse>> Get(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole(UserRole.Admin);
        return Ok(await dashboardService.GetAsync(userId, isAdmin, cancellationToken));
    }
}