// Controllers/Admin/AdminAuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/auth")]
    [Produces("application/json")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAdminAuthService _adminAuthService;

        public AdminAuthController(IAdminAuthService adminAuthService)
        {
            _adminAuthService = adminAuthService;
        }

        /// <summary>
        /// Admin login
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token and admin profile</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AdminLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _adminAuthService.LoginAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Create new admin (Super admin only)
        /// </summary>
        /// <param name="request">Admin creation data</param>
        /// <returns>Created admin profile</returns>
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateAdmin([FromBody] AdminCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if current user is super admin
            var isSuperAdmin = User.FindFirst("IsSuperAdmin")?.Value == "True";
            if (!isSuperAdmin)
            {
                return Forbid("Only super admin can create new admins");
            }

            var result = await _adminAuthService.CreateAdminAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Admin logout
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var adminId = GetCurrentAdminId();
            var result = await _adminAuthService.LogoutAsync(adminId);

            return Ok(result);
        }

        /// <summary>
        /// Get admin profile
        /// </summary>
        /// <returns>Admin profile details</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var adminId = GetCurrentAdminId();
            var result = await _adminAuthService.GetProfileAsync(adminId);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Update admin profile
        /// </summary>
        /// <param name="request">Profile update data</param>
        /// <returns>Updated profile</returns>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] AdminProfileUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminId = GetCurrentAdminId();
            var result = await _adminAuthService.UpdateProfileAsync(adminId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Change admin password
        /// </summary>
        /// <param name="request">Password change data</param>
        /// <returns>Success message</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] AdminChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminId = GetCurrentAdminId();
            var result = await _adminAuthService.ChangePasswordAsync(adminId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        private int GetCurrentAdminId()
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(adminIdClaim ?? "0");
        }
    }
}