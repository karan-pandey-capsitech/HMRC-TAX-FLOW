using HMRC_TAX_FLOW.Application.Authentication.DTOs;
using HMRC_TAX_FLOW.Application.Users;
using HMRC_TAX_FLOW.Application.Users.DTOs;
using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMRC_TAX_FLOW.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = UserRole.Admin)]
public sealed class UsersController(IUserService userService) : ControllerBase
{
    [HttpPost("{id:guid}/role")]
    public async Task<ActionResult<UserProfileResponse>> AssignRole(
        Guid id,
        AssignUserRoleRequest request,
        CancellationToken cancellationToken)
    {
        var user = await userService.AssignRoleAsync(id, request.Role, cancellationToken);
        return Ok(new UserProfileResponse(
            user.Id,
            user.Username,
            user.Email,
            user.FullName,
            user.Roles.AsReadOnly()));
    }
}
