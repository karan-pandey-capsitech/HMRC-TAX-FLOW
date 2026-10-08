using System.Security.Claims;
using HMRC_TAX_FLOW.Application.SA100;
using HMRC_TAX_FLOW.Application.SA100.DTOs;
using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMRC_TAX_FLOW.Controllers;

[ApiController]
[Route("api/sa100")]
[Authorize(Roles = $"{UserRole.Admin},{UserRole.Practice}")]
public sealed class Sa100Controller(ISa100Service sa100Service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Sa100Response>>> GetAll(CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return Ok(await sa100Service.GetAllAsync(
            userId,
            User.IsInRole(UserRole.Admin),
            cancellationToken));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Sa100Response>> GetById(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return Ok(await sa100Service.GetByIdAsync(
            id,
            userId,
            User.IsInRole(UserRole.Admin),
            cancellationToken));
    }

    [Authorize(Roles = UserRole.Practice)]
    [HttpPost]
    public async Task<ActionResult<Sa100Response>> Create(
        CreateSa100Request request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        var result = await sa100Service.CreateAsync(request, userId, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [Authorize(Roles = UserRole.Practice)]
    [HttpPost("{id:guid}")]
    public async Task<ActionResult<Sa100Response>> Update(
        Guid id,
        UpdateSa100Request request,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return Ok(await sa100Service.UpdateDraftAsync(id, request, userId, cancellationToken));
    }

    [Authorize(Roles = UserRole.Practice)]
    [HttpPost("{id:guid}/submit")]
    public async Task<ActionResult<Sa100Response>> Submit(Guid id, CancellationToken cancellationToken)
    {
        if (!TryGetUserId(out var userId))
        {
            return Unauthorized();
        }

        return Ok(await sa100Service.SubmitAsync(id, userId, cancellationToken));
    }

    private bool TryGetUserId(out Guid userId) =>
        Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
}