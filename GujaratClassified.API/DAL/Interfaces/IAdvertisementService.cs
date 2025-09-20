using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IAdvertisementService
    {
        // Advertisement Management (Admin)
        Task<ApiResponse<int>> CreateAdvertisementAsync(CreateAdvertisementRequest request);
        Task<ApiResponse<AdvertisementResponse>> GetAdvertisementByIdAsync(int adId);
        Task<ApiResponse<(List<AdListResponse> Ads, PaginationResponse Pagination)>> GetAdvertisementsWithFiltersAsync(AdFilterRequest filter);
        Task<ApiResponse<bool>> UpdateAdvertisementAsync(int adId, UpdateAdvertisementRequest request);
        Task<ApiResponse<bool>> UpdateAdvertisementStatusAsync(int adId, AdStatusRequest request);
        Task<ApiResponse<bool>> DeleteAdvertisementAsync(int adId);

        // Advertisement Media Management
        Task<ApiResponse<bool>> UploadAdvertisementImageAsync(int adId, IFormFile imageFile);
        Task<ApiResponse<bool>> UploadAdvertisementVideoAsync(int adId, IFormFile videoFile);

        // Advertisement Analytics
        Task<ApiResponse<AdAnalyticsResponse>> GetAdvertisementAnalyticsAsync(int adId);
        Task<ApiResponse<bool>> TrackAdViewAsync(int adId, int? userId, string? ipAddress, string? userAgent, string position);
        Task<ApiResponse<bool>> TrackAdClickAsync(int adId, int? userId, string? ipAddress, string? userAgent, string? referrer);

        // Advertisement Inquiries Management
        Task<ApiResponse<(List<AdInquiryResponse> Inquiries, PaginationResponse Pagination)>> GetAdInquiriesAsync(int? adId = null, string? status = null, int pageSize = 20, int pageNumber = 1);
        Task<ApiResponse<bool>> UpdateInquiryStatusAsync(int inquiryId, string status);
    }
}