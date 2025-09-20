using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IPublicAdvertisementService
    {
        // Public Advertisement Display (Mobile)
        Task<ApiResponse<List<PublicAdResponse>>> GetAdvertisementsByPositionAsync(string position, int? userDistrictId = null, int? userCategoryId = null, string? userGender = null, int? userAge = null);
        Task<ApiResponse<PublicAdResponse>> GetPublicAdvertisementByIdAsync(int adId);

        // User Advertisement Interactions
        Task<ApiResponse<int>> CreateAdInquiryAsync(AdInquiryRequest request, int userId);
        Task<ApiResponse<List<AdInquiryResponse>>> GetUserAdInquiriesAsync(int userId, int pageSize = 20, int pageNumber = 1);

        // Advertisement Tracking
        Task<ApiResponse<bool>> TrackAdViewAsync(int adId, int? userId, string? ipAddress, string? userAgent, string position);
        Task<ApiResponse<bool>> TrackAdClickAsync(int adId, int? userId, string? ipAddress, string? userAgent, string? referrer);
    }
}