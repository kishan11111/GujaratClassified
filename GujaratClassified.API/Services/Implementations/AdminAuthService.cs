using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Helpers;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Services.Implementations
{
    public class AdminAuthService : IAdminAuthService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly IJwtHelper _jwtHelper;

        public AdminAuthService(IAdminRepository adminRepository, IJwtHelper jwtHelper)
        {
            _adminRepository = adminRepository;
            _jwtHelper = jwtHelper;
        }

        public async Task<ApiResponse<AdminLoginResponse>> LoginAsync(AdminLoginRequest request)
        {
            try
            {
                // Get admin by email
                var admin = await _adminRepository.GetAdminByEmailAsync(request.Email);
                if (admin == null)
                {
                    return ApiResponse<AdminLoginResponse>.ErrorResponse("Invalid email or password");
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
                {
                    return ApiResponse<AdminLoginResponse>.ErrorResponse("Invalid email or password");
                }

                // Generate tokens
                var accessToken = _jwtHelper.GenerateAccessToken(admin);
                var refreshToken = _jwtHelper.GenerateRefreshToken();
                var expiresAt = DateTime.UtcNow.AddMinutes(_jwtHelper.GetAccessTokenExpirationMinutes());

                // Update refresh token in database
                await _adminRepository.UpdateRefreshTokenAsync(admin.AdminId, refreshToken,
                    DateTime.UtcNow.AddDays(_jwtHelper.GetRefreshTokenExpirationDays()));

                // Update last login
                await _adminRepository.UpdateLastLoginAsync(admin.AdminId);

                var response = new AdminLoginResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    ExpiresAt = expiresAt,
                    Admin = new AdminProfileResponse
                    {
                        AdminId = admin.AdminId,
                        FirstName = admin.FirstName,
                        LastName = admin.LastName,
                        Email = admin.Email,
                        Mobile = admin.Mobile,
                        ProfileImage = admin.ProfileImage,
                        IsSuperAdmin = admin.IsSuperAdmin,
                        LastLoginAt = admin.LastLoginAt,
                        CreatedAt = admin.CreatedAt
                    }
                };

                return ApiResponse<AdminLoginResponse>.SuccessResponse(response, "Login successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<AdminLoginResponse>.ErrorResponse("An error occurred during login",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> LogoutAsync(int adminId)
        {
            try
            {
                // Clear refresh token
                await _adminRepository.UpdateRefreshTokenAsync(adminId, "", DateTime.UtcNow.AddDays(-1));

                return ApiResponse<object>.SuccessResponse(null, "Logout successful");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse("An error occurred during logout",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AdminProfileResponse>> GetProfileAsync(int adminId)
        {
            try
            {
                var admin = await _adminRepository.GetAdminByIdAsync(adminId);
                if (admin == null)
                {
                    return ApiResponse<AdminProfileResponse>.ErrorResponse("Admin not found");
                }

                var profile = new AdminProfileResponse
                {
                    AdminId = admin.AdminId,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Email = admin.Email,
                    Mobile = admin.Mobile,
                    ProfileImage = admin.ProfileImage,
                    IsSuperAdmin = admin.IsSuperAdmin,
                    LastLoginAt = admin.LastLoginAt,
                    CreatedAt = admin.CreatedAt
                };

                return ApiResponse<AdminProfileResponse>.SuccessResponse(profile);
            }
            catch (Exception ex)
            {
                return ApiResponse<AdminProfileResponse>.ErrorResponse("An error occurred while fetching profile",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AdminProfileResponse>> UpdateProfileAsync(int adminId, AdminProfileUpdateRequest request)
        {
            try
            {
                var admin = await _adminRepository.GetAdminByIdAsync(adminId);
                if (admin == null)
                {
                    return ApiResponse<AdminProfileResponse>.ErrorResponse("Admin not found");
                }

                // Update admin details
                admin.FirstName = request.FirstName;
                admin.LastName = request.LastName;
                admin.Mobile = request.Mobile;
                admin.ProfileImage = request.ProfileImage;

                var updateResult = await _adminRepository.UpdateAdminAsync(admin);
                if (!updateResult)
                {
                    return ApiResponse<AdminProfileResponse>.ErrorResponse("Failed to update profile");
                }

                var profile = new AdminProfileResponse
                {
                    AdminId = admin.AdminId,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Email = admin.Email,
                    Mobile = admin.Mobile,
                    ProfileImage = admin.ProfileImage,
                    IsSuperAdmin = admin.IsSuperAdmin,
                    LastLoginAt = admin.LastLoginAt,
                    CreatedAt = admin.CreatedAt
                };

                return ApiResponse<AdminProfileResponse>.SuccessResponse(profile, "Profile updated successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AdminProfileResponse>.ErrorResponse("An error occurred while updating profile",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> ChangePasswordAsync(int adminId, AdminChangePasswordRequest request)
        {
            try
            {
                var admin = await _adminRepository.GetAdminByIdAsync(adminId);
                if (admin == null)
                {
                    return ApiResponse<object>.ErrorResponse("Admin not found");
                }

                // Verify current password
                if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, admin.PasswordHash))
                {
                    return ApiResponse<object>.ErrorResponse("Current password is incorrect");
                }

                // Update password
                admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

                var updateResult = await _adminRepository.UpdateAdminAsync(admin);
                if (!updateResult)
                {
                    return ApiResponse<object>.ErrorResponse("Failed to change password");
                }

                return ApiResponse<object>.SuccessResponse(null, "Password changed successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<object>.ErrorResponse("An error occurred while changing password",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AdminProfileResponse>> CreateAdminAsync(AdminCreateRequest request)
        {
            try
            {
                // Check if email already exists
                var emailExists = await _adminRepository.IsEmailExistsAsync(request.Email);
                if (emailExists)
                {
                    return ApiResponse<AdminProfileResponse>.ErrorResponse("Email already exists");
                }

                // Create new admin
                var admin = new Admin
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Mobile = request.Mobile,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    IsSuperAdmin = request.IsSuperAdmin,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var adminId = await _adminRepository.CreateAdminAsync(admin);
                admin.AdminId = adminId;

                var profile = new AdminProfileResponse
                {
                    AdminId = admin.AdminId,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    Email = admin.Email,
                    Mobile = admin.Mobile,
                    ProfileImage = admin.ProfileImage,
                    IsSuperAdmin = admin.IsSuperAdmin,
                    LastLoginAt = admin.LastLoginAt,
                    CreatedAt = admin.CreatedAt
                };

                return ApiResponse<AdminProfileResponse>.SuccessResponse(profile, "Admin created successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<AdminProfileResponse>.ErrorResponse("An error occurred while creating admin",
                    new List<string> { ex.Message });
            }
        }
    }
}