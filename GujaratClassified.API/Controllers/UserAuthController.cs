// Controllers/UserAuthController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.DAL.Interfaces;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/user/auth")]
    [Produces("application/json")]
    public class UserAuthController : ControllerBase
    {
        private readonly IUserAuthService _userAuthService;
        private readonly IPreRegistrationRepository _preRegistrationRepository;
        private readonly ILogger<UserAuthController> _logger;


        public UserAuthController(IUserAuthService userAuthService,
            IPreRegistrationRepository preRegistrationRepository,
            ILogger<UserAuthController> logger)
        {
            _userAuthService = userAuthService;
            _preRegistrationRepository = preRegistrationRepository;
            _logger = logger;
        }

        /// <summary>
        /// Pre-register user without OTP (for app launch pre-registration)
        /// </summary>
        /// <param name="request">Name, mobile, and optional village</param>
        /// <returns>Success confirmation</returns>
        [HttpPost("pre-register")]
        public async Task<IActionResult> PreRegister([FromBody] PreRegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Success = false, Message = "Invalid request data", Errors = ModelState });
            }

            try
            {
                // Check if mobile already registered
                var exists = await _preRegistrationRepository.ExistsByMobileAsync(request.Mobile);
                if (exists)
                {
                    return Ok(new { Success = true, Message = "You are already pre-registered! We will contact you soon." });
                }

                // Create pre-registration
                var preRegistration = new PreRegistration
                {
                    Name = request.Name,
                    Mobile = request.Mobile,
                    DistrictId = request.DistrictId,
                    TalukaId = request.TalukaId,
                    VillageId = request.VillageId,
                    CreatedAt = DateTime.UtcNow,
                    IsConverted = false
                };

                var result = await _preRegistrationRepository.CreateAsync(preRegistration);

                if (result)
                {
                    _logger.LogInformation("Pre-registration successful for mobile: {Mobile}", request.Mobile);
                    return Ok(new { Success = true, Message = "Pre-registration successful! We will contact you when the app launches." });
                }

                return BadRequest(new { Success = false, Message = "Failed to save pre-registration. Please try again." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during pre-registration for mobile: {Mobile}", request.Mobile);
                return StatusCode(500, new { Success = false, Message = "An error occurred. Please try again later." });
            }
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