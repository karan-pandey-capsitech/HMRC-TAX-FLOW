using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HMRC_TAX_FLOW.Application.Authentication;
using HMRC_TAX_FLOW.Application.Authentication.DTOs;
using HMRC_TAX_FLOW.Application.Users;
using HMRC_TAX_FLOW.Domain.Users;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var user = await _auth.RegisterAsync(req);
            return CreatedAtAction(nameof(GetProfile), new { id = user.Id }, new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Roles
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var token = await _auth.LoginAsync(req);
            return Ok(token);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var id = GetUserId();
            if (id == Guid.Empty) return Unauthorized();

            var user = await _users.GetByIdAsync(id);
            if (user == null) return NotFound();

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.FullName,
                user.Roles
            });
        }

        [Authorize]
        [HttpPost("profile/update")]
        public async Task<IActionResult> UpdateProfile([FromBody] RegisterRequest req)
        {
            var id = GetUserId();
            if (id == Guid.Empty) return Unauthorized();

            var user = await _users.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = req.Email ?? user.Email;
            user.FullName = req.FullName ?? user.FullName;

            if (!string.IsNullOrWhiteSpace(req.Role) && User.IsInRole(UserRole.Admin))
            {
                user.Roles = new System.Collections.Generic.List<string> { req.Role! };
            }

            var updated = await _users.UpdateAsync(user);
            return Ok(new { updated.Id, updated.Username, updated.Email, updated.FullName, updated.Roles });
        }

        [Authorize]
        [HttpPost("profile/delete")]
        public async Task<IActionResult> DeleteProfile()
        {
            var id = GetUserId();
            if (id == Guid.Empty) return Unauthorized();

            await _users.DeleteAsync(id);
            return Ok(new { deleted = true });
        }

        private Guid GetUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idClaim, out var id) ? id : Guid.Empty;
        }
    }
}