using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IAdminAuthService
    {
        Task<ApiResponse<AdminLoginResponse>> LoginAsync(AdminLoginRequest request);
        Task<ApiResponse<object>> LogoutAsync(int adminId);
        Task<ApiResponse<AdminProfileResponse>> GetProfileAsync(int adminId);
        Task<ApiResponse<AdminProfileResponse>> UpdateProfileAsync(int adminId, AdminProfileUpdateRequest request);
        Task<ApiResponse<object>> ChangePasswordAsync(int adminId, AdminChangePasswordRequest request);
        Task<ApiResponse<AdminProfileResponse>> CreateAdminAsync(AdminCreateRequest request);
    }
}
