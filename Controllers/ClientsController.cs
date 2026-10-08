using System.Security.Claims;
using HMRC_TAX_FLOW.Application.Clients;
using HMRC_TAX_FLOW.Application.Clients.DTOs;
using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMRC_TAX_FLOW.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize(Roles = $"{UserRole.Admin},{UserRole.Practice},{UserRole.Debitam}")]
public sealed class ClientsController(IClientService clientService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ClientResponse>>> GetAll(CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        return Ok(await clientService.GetAllAsync(
            userId,
            User.IsInRole(UserRole.Admin),
            cancellationToken));
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Create(
        CreateClientRequest request,
        CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized();
        }

        var client = await clientService.CreateAsync(request, userId, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, client);
    }
}
