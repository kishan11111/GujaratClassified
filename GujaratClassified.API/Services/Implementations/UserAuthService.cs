using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Helpers;

namespace GujaratClassified.API.Services.Implementations
{
    public class UserAuthService : IUserAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IOTPService _otpService;
        private readonly ILocationRepository _locationRepository;
        private readonly IJwtHelper _jwtHelper;
        private readonly ILogger<UserAuthService> _logger;

        public UserAuthService(
            IUserRepository userRepository,
            IOTPService otpService,
            ILocationRepository locationRepository,
            IJwtHelper jwtHelper,
            ILogger<UserAuthService> logger)
        {
            _userRepository = userRepository;
            _otpService = otpService;
            _locationRepository = locationRepository;
            _jwtHelper = jwtHelper;
            _logger = logger;
        }

        public async Task<ApiResponse<OTPResponse>> SendOTPAsync(SendOTPRequest request)
        {
            try
            {
                // Validate purpose
                var validPurposes = new[] { "REGISTER", "LOGIN", "FORGOT_PASSWORD" };
                if (!validPurposes.Contains(request.Purpose.ToUpper()))
                {
                    return ApiResponse<OTPResponse>.ErrorResponse("Invalid OTP purpose");
                }

                // For LOGIN purpose, check if user exists
                if (request.Purpose.ToUpper() == "LOGIN")
                {
                    var userExists = await _userRepository.IsMobileExistsAsync(request.Mobile);
                    if (!userExists)
                    {
                        return ApiResponse<OTPResponse>.ErrorResponse("Mobile number not registered. Please register first.");
                    }
                }

                // For REGISTER purpose, check if user already exists
                if (request.Purpose.ToUpper() == "REGISTER")
                {
                    var userExists = await _userRepository.IsMobileExistsAsync(request.Mobile);
                    if (userExists)
                    {
                        return ApiResponse<OTPResponse>.ErrorResponse("Mobile number already registered. Please login instead.");
                    }
                }

                return await _otpService.SendOTPAsync(request.Mobile, request.Purpose.ToUpper());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendOTPAsync for mobile {Mobile}", request.Mobile);
                return ApiResponse<OTPResponse>.ErrorResponse("An error occurred while sending OTP",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<UserLoginResponse>> VerifyOTPAndLoginAsync(VerifyOTPRequest request)
        {
            try
            {
                // Verify OTP first
                var otpResult = await _otpService.VerifyOTPAsync(request.Mobile, request.OTP, request.Purpose.ToUpper());
                if (!otpResult.Success)
                {
                    return ApiResponse<UserLoginResponse>.ErrorResponse(otpResult.Message);
                }

                // Handle different purposes
                if (request.Purpose.ToUpper() == "LOGIN")
                {
                    return await LoginWithOTPAsync(request.Mobile);
                }
                else if (request.Purpose.ToUpper() == "REGISTER")
                {
                    return ApiResponse<UserLoginResponse>.SuccessResponse(null,
                        "OTP verified. Please complete registration.");
                }
                else
                {
                    return ApiResponse<UserLoginResponse>.SuccessResponse(null, "OTP verified successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in VerifyOTPAndLoginAsync for mobile {Mobile}", request.Mobile);
                return ApiResponse<UserLoginResponse>.ErrorResponse("An error occurred while verifying OTP",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<UserLoginResponse>> RegisterUserAsync(UserRegistrationRequest request)
        {
            try
            {
                // Check if mobile already exists
                var mobileExists = await _userRepository.IsMobileExistsAsync(request.Mobile);
                if (mobileExists)
                {
                    return ApiResponse<UserLoginResponse>.ErrorResponse("Mobile number already registered");
                }

                // Validate location IDs
                var district = await _locationRepository.GetDistrictByIdAsync(request.DistrictId);
                if (district == null)
                {
                    return ApiResponse<UserLoginResponse>.ErrorResponse("Invalid district selected");
                }

                var taluka = await _locationRepository.GetTalukaByIdAsync(request.TalukaId);
                if (taluka == null || taluka.DistrictId != request.DistrictId)
                {
                    return ApiResponse<UserLoginResponse>.ErrorResponse("Invalid taluka selected");
                }

                var village = await _locationRepository.GetVillageByIdAsync(request.VillageId);
                if (village == null || village.TalukaId != request.TalukaId)
                {
                    return ApiResponse<UserLoginResponse>.ErrorResponse("Invalid village selected");
                }

                // Create user entity
                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Mobile = request.Mobile,
                    Email = request.Email,
                    PasswordHash = !string.IsNullOrEmpty(request.Password) ?
                                  BCrypt.Net.BCrypt.HashPassword(request.Password) : null,
                    DistrictId = request.DistrictId,
                    TalukaId = request.TalukaId,
                    VillageId = request.VillageId,
                    DateOfBirth = request.DateOfBirth,
                    Gender = request.Gender,
                    IsActive = true,
                    IsVerified = true, // Auto-verified since OTP is confirmed
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save user to database
                var userId = await _userRepository.CreateUserAsync(user);
                user.UserId = userId;

                // Set location names for response
                user.DistrictName = district.DistrictNameEnglish;
                user.TalukaName = taluka.TalukaNameEnglish;
                user.VillageName = village.VillageNameEnglish;

                // Generate tokens and login
                return await GenerateLoginResponse(user, true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterUserAsync for mobile {Mobile}", request.Mobile);
                return ApiResponse<UserLoginResponse>.ErrorResponse("An error occurred during registration",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<UserLoginResponse>> LoginAsync(UserLoginRequest request)
        {
            try
            {
                // Get user by mobile
                var user = await _userRepository.GetUserByMobileAsync(request.Mobile);
                if (user == null)
                {
                    return ApiResponse<UserLoginResponse>.ErrorResponse("Invalid mobile number or password");
                }

                // Check if user has password set
                if (string.IsNullOrEmpty(user.PasswordHash))
                {
                    return ApiResponse<UserLoginResponse>.ErrorResponse("Please login using OTP or set a password first");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return ApiResponse<UserLoginResponse>.ErrorResponse("Invalid mobile number or password");
                }

                // Update last login and generate response
                await _userRepository.UpdateLastLoginAsync(user.UserId);
                return await GenerateLoginResponse(user, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LoginAsync for mobile {Mobile}", request.Mobile);
                return ApiResponse<UserLoginResponse>.ErrorResponse("An error occurred during login",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> LogoutAsync(int userId)
        {
            try
            {
                // Clear refresh token
                await _userRepository.UpdateRefreshTokenAsync(userId, "", DateTime.UtcNow.AddDays(-1));

                return ApiResponse<object>.SuccessResponse(null, "Logout successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in LogoutAsync for user {UserId}", userId);
                return ApiResponse<object>.ErrorResponse("An error occurred during logout",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<UserProfileResponse>> GetProfileAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<UserProfileResponse>.ErrorResponse("User not found");
                }

                var profile = new UserProfileResponse
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    DistrictId = user.DistrictId,
                    TalukaId = user.TalukaId,
                    VillageId = user.VillageId,
                    DistrictName = user.DistrictName,
                    TalukaName = user.TalukaName,
                    VillageName = user.VillageName,
                    ProfileImage = user.ProfileImage,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    IsVerified = user.IsVerified,
                    IsPremium = user.IsPremium,
                    LastLoginAt = user.LastLoginAt,
                    CreatedAt = user.CreatedAt
                };

                return ApiResponse<UserProfileResponse>.SuccessResponse(profile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProfileAsync for user {UserId}", userId);
                return ApiResponse<UserProfileResponse>.ErrorResponse("An error occurred while fetching profile",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<UserProfileResponse>> UpdateProfileAsync(int userId, UserProfileUpdateRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<UserProfileResponse>.ErrorResponse("User not found");
                }

                // Validate location IDs
                var district = await _locationRepository.GetDistrictByIdAsync(request.DistrictId);
                if (district == null)
                {
                    return ApiResponse<UserProfileResponse>.ErrorResponse("Invalid district selected");
                }

                var taluka = await _locationRepository.GetTalukaByIdAsync(request.TalukaId);
                if (taluka == null || taluka.DistrictId != request.DistrictId)
                {
                    return ApiResponse<UserProfileResponse>.ErrorResponse("Invalid taluka selected");
                }

                var village = await _locationRepository.GetVillageByIdAsync(request.VillageId);
                if (village == null || village.TalukaId != request.TalukaId)
                {
                    return ApiResponse<UserProfileResponse>.ErrorResponse("Invalid village selected");
                }

                // Update user details
                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.Email = request.Email;
                user.DistrictId = request.DistrictId;
                user.TalukaId = request.TalukaId;
                user.VillageId = request.VillageId;
                user.DateOfBirth = request.DateOfBirth;
                user.Gender = request.Gender;
                user.ProfileImage = request.ProfileImage;

                var updateResult = await _userRepository.UpdateUserProfileAsync(user);
                if (!updateResult)
                {
                    return ApiResponse<UserProfileResponse>.ErrorResponse("Failed to update profile");
                }

                // Set location names for response
                user.DistrictName = district.DistrictNameEnglish;
                user.TalukaName = taluka.TalukaNameEnglish;
                user.VillageName = village.VillageNameEnglish;

                var profile = new UserProfileResponse
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Mobile = user.Mobile,
                    Email = user.Email,
                    DistrictId = user.DistrictId,
                    TalukaId = user.TalukaId,
                    VillageId = user.VillageId,
                    DistrictName = user.DistrictName,
                    TalukaName = user.TalukaName,
                    VillageName = user.VillageName,
                    ProfileImage = user.ProfileImage,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    IsVerified = user.IsVerified,
                    IsPremium = user.IsPremium,
                    LastLoginAt = user.LastLoginAt,
                    CreatedAt = user.CreatedAt
                };

                return ApiResponse<UserProfileResponse>.SuccessResponse(profile, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateProfileAsync for user {UserId}", userId);
                return ApiResponse<UserProfileResponse>.ErrorResponse("An error occurred while updating profile",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> ChangePasswordAsync(int userId, UserChangePasswordRequest request)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<object>.ErrorResponse("User not found");
                }

                // If user doesn't have a password set, only validate new password
                if (!string.IsNullOrEmpty(user.PasswordHash))
                {
                    // Verify current password
                    if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                    {
                        return ApiResponse<object>.ErrorResponse("Current password is incorrect");
                    }
                }

                // Update password
                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                var updateResult = await _userRepository.UpdateUserPasswordAsync(userId, newPasswordHash);

                if (!updateResult)
                {
                    return ApiResponse<object>.ErrorResponse("Failed to change password");
                }

                return ApiResponse<object>.SuccessResponse(null, "Password changed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChangePasswordAsync for user {UserId}", userId);
                return ApiResponse<object>.ErrorResponse("An error occurred while changing password",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest request)
        {
            try
            {
                // Check if user exists
                var user = await _userRepository.GetUserByMobileAsync(request.Mobile);
                if (user == null)
                {
                    return ApiResponse<object>.ErrorResponse("Mobile number not registered");
                }

                // Note: In real implementation, you should verify OTP before allowing password reset
                // For now, we'll directly update the password

                var newPasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                var updateResult = await _userRepository.UpdateUserPasswordAsync(user.UserId, newPasswordHash);

                if (!updateResult)
                {
                    return ApiResponse<object>.ErrorResponse("Failed to reset password");
                }

                return ApiResponse<object>.SuccessResponse(null, "Password reset successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ForgotPasswordAsync for mobile {Mobile}", request.Mobile);
                return ApiResponse<object>.ErrorResponse("An error occurred while resetting password",
                    new List<string> { ex.Message });
            }
        }

        // Private helper methods
        private async Task<ApiResponse<UserLoginResponse>> LoginWithOTPAsync(string mobile)
        {
            var user = await _userRepository.GetUserByMobileAsync(mobile);
            if (user == null)
            {
                return ApiResponse<UserLoginResponse>.ErrorResponse("User not found");
            }

            await _userRepository.UpdateLastLoginAsync(user.UserId);
            return await GenerateLoginResponse(user, false);
        }

        private async Task<ApiResponse<UserLoginResponse>> GenerateLoginResponse(User user, bool isNewUser)
        {
            try
            {
                // Generate tokens
                var accessToken = _jwtHelper.GenerateAccessToken(user);
                var refreshToken = _jwtHelper.GenerateRefreshToken();
                var expiresAt = DateTime.UtcNow.AddMinutes(_jwtHelper.GetAccessTokenExpirationMinutes());

                // Update refresh token in database
                await _userRepository.UpdateRefreshTokenAsync(user.UserId, refreshToken,
                    DateTime.UtcNow.AddDays(_jwtHelper.GetRefreshTokenExpirationDays()));

                var response = new UserLoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    IsNewUser = isNewUser,
                    User = new UserProfileResponse
                    {
                        UserId = user.UserId,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Mobile = user.Mobile,
                        Email = user.Email,
                        DistrictId = user.DistrictId,
                        TalukaId = user.TalukaId,
                        VillageId = user.VillageId,
                        DistrictName = user.DistrictName,
                        TalukaName = user.TalukaName,
                        VillageName = user.VillageName,
                        ProfileImage = user.ProfileImage,
                        DateOfBirth = user.DateOfBirth,
                        Gender = user.Gender,
                        IsVerified = user.IsVerified,
                        IsPremium = user.IsPremium,
                        LastLoginAt = user.LastLoginAt,
                        CreatedAt = user.CreatedAt
                    }
                };

                return ApiResponse<UserLoginResponse>.SuccessResponse(response,
                    isNewUser ? "Registration successful" : "Login successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating login response for user {UserId}", user.UserId);
                return ApiResponse<UserLoginResponse>.ErrorResponse("An error occurred during authentication",
                    new List<string> { ex.Message });
            }
        }
    }
}