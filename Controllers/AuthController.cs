using System.Security.Claims;
using HMRC_TAX_FLOW.Application.Authentication;
using HMRC_TAX_FLOW.Application.Authentication.DTOs;
using HMRC_TAX_FLOW.Application.Users;
using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMRC_TAX_FLOW.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;
        private readonly IUserService _users;

        public AuthController(IAuthService auth, IUserService users)
        {
            _auth = auth;
            _users = users;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromBody] RegisterRequest req,
            CancellationToken cancellationToken)
        {
            var user = await _auth.RegisterAsync(req, cancellationToken);
            return StatusCode(StatusCodes.Status201Created, ToProfileResponse(user));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest req,
            CancellationToken cancellationToken)
        {
            var token = await _auth.LoginAsync(req, cancellationToken);
            return Ok(token);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            var id = GetUserId();
            if (id == Guid.Empty) return Unauthorized();

            var user = await _users.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();

            return Ok(ToProfileResponse(user));
        }

        [Authorize]
        [HttpPost("profile/update")]
        public async Task<IActionResult> UpdateProfile(
            [FromBody] UpdateProfileRequest req,
            CancellationToken cancellationToken)
        {
            var id = GetUserId();
            if (id == Guid.Empty) return Unauthorized();

            var user = await _users.GetByIdAsync(id, cancellationToken);
            if (user == null) return NotFound();

            if (req.Email is not null)
            {
                user.Email = req.Email.Trim();
            }

            if (req.FullName is not null)
            {
                user.FullName = string.IsNullOrWhiteSpace(req.FullName)
                    ? null
                    : req.FullName.Trim();
            }

            var updated = await _users.UpdateAsync(user, cancellationToken);
            return updated is null ? NotFound() : Ok(ToProfileResponse(updated));
        }

        [Authorize]
        [HttpPost("profile/delete")]
        public async Task<IActionResult> DeleteProfile(CancellationToken cancellationToken)
        {
            var id = GetUserId();
            if (id == Guid.Empty) return Unauthorized();

            var deleted = await _users.DeleteAsync(id, cancellationToken);
            if (!deleted) return NotFound();

            return Ok(new { deleted = true });
        }

        private Guid GetUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
        }

        private static UserProfileResponse ToProfileResponse(User user) =>
            new(user.Id, user.Username, user.Email, user.FullName, user.Roles.AsReadOnly());
    }
}