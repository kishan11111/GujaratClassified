// Controllers/UserAuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/user/auth")]
    [Produces("application/json")]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;
        private readonly ILogger<UserAuthController> _logger;


        public UserAuthController(IUserAuthService userAuthService,
            ILogger<UserAuthController> logger)
        {
            _userAuthService = userAuthService;
            _logger = logger;
        }

        /// <summary>
        /// Send OTP for registration, login, or password reset
        /// </summary>
        /// <param name="request">Mobile number and purpose</param>
        /// <returns>OTP sent confirmation</returns>
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOTP([FromBody] SendOTPRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userAuthService.SendOTPAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Verify OTP and proceed with login (if user exists)
        /// </summary>
        /// <param name="request">Mobile, OTP, and purpose</param>
        /// <returns>Login response or verification confirmation</returns>
        //[HttpPost("verify-otp")]
        //public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequest request)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var result = await _userAuthService.VerifyOTPAndLoginAsync(request);

        //    if (result.Success)
        //    {
        //        return Ok(result);
        //    }

        //    return BadRequest(result);
        //}

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            _logger.LogInformation("=== OTP Verification Started ===");
            _logger.LogInformation("Request received - Mobile: {Mobile}, Purpose: {Purpose}",
                request?.Mobile, request?.Purpose);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid ModelState for OTP verification. Errors: {Errors}",
                    string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _userAuthService.VerifyOTPAndLoginAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("OTP verification successful for mobile: {Mobile}", request.Mobile);
                    return Ok(result);
                }

                _logger.LogWarning("OTP verification failed for mobile: {Mobile}. Reason: {Message}",
                    request.Mobile, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in VerifyOTP controller for mobile: {Mobile}",
                    request?.Mobile);
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "An unexpected error occurred",
                    Error = ex.Message
                });
            }
            finally
            {
                _logger.LogInformation("=== OTP Verification Completed ===");
            }
        }

        /// <summary>
        /// Complete user registration after OTP verification
        /// </summary>
        /// <param name="request">User registration details</param>
        /// <returns>JWT token and user profile</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userAuthService.RegisterUserAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// User login with mobile and password
        /// </summary>
        /// <param name="request">Login credentials</param>
        /// <returns>JWT token and user profile</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userAuthService.LoginAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// User logout
        /// </summary>
        /// <returns>Success message</returns>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = GetCurrentUserId();
            var result = await _userAuthService.LogoutAsync(userId);

            return Ok(result);
        }

        /// <summary>
        /// Get user profile
        /// </summary>
        /// <returns>User profile details</returns>
        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = GetCurrentUserId();
            var result = await _userAuthService.GetProfileAsync(userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <param name="request">Profile update data</param>
        /// <returns>Updated profile</returns>
        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileUpdateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _userAuthService.UpdateProfileAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Change user password
        /// </summary>
        /// <param name="request">Password change data</param>
        /// <returns>Success message</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] UserChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _userAuthService.ChangePasswordAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Reset password using OTP (should be called after OTP verification)
        /// </summary>
        /// <param name="request">New password details</param>
        /// <returns>Success message</returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _userAuthService.ForgotPasswordAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}