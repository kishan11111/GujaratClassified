using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IUserAuthService
    {
        Task<ApiResponse<OTPResponse>> SendOTPAsync(SendOTPRequest request);
        Task<ApiResponse<UserLoginResponse>> VerifyOTPAndLoginAsync(VerifyOTPRequest request);
        Task<ApiResponse<UserLoginResponse>> RegisterUserAsync(UserRegistrationRequest request);
        Task<ApiResponse<UserLoginResponse>> LoginAsync(UserLoginRequest request);
        Task<ApiResponse<object>> LogoutAsync(int userId);
        Task<ApiResponse<UserProfileResponse>> GetProfileAsync(int userId);
        Task<ApiResponse<UserProfileResponse>> UpdateProfileAsync(int userId, UserProfileUpdateRequest request);
        Task<ApiResponse<object>> ChangePasswordAsync(int userId, UserChangePasswordRequest request);
        Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest request);
    }
}
