using GujaratClassified.API.Models.Common;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IFarmerProfileService
    {
        Task<ApiResponse<FarmerProfileResponse>> CreateFarmerProfileAsync(int userId, CreateFarmerProfileRequest request);
        Task<ApiResponse<FarmerProfileResponse>> GetFarmerProfileAsync(int userId);
        Task<ApiResponse<FarmerProfileResponse>> UpdateFarmerProfileAsync(int userId, CreateFarmerProfileRequest request);
        Task<ApiResponse<List<TopFarmerResponse>>> GetTopFarmersAsync(int limit = 10, string orderBy = "TotalLikes");
        Task<ApiResponse<bool>> SetVerifiedFarmerAsync(int userId, bool isVerified);
    }
}
