using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAdvertisementRepository
    {
        // Advertisement Management
        Task<int> CreateAdvertisementAsync(Advertisement advertisement);
        Task<Advertisement?> GetAdvertisementByIdAsync(int adId);
        Task<bool> UpdateAdvertisementAsync(Advertisement advertisement);
        Task<bool> UpdateAdvertisementStatusAsync(int adId, string status);
        Task<bool> DeleteAdvertisementAsync(int adId);

        // Advertisement Lists with Filters
        Task<(List<Advertisement> Ads, int TotalCount)> GetAdvertisementsWithFiltersAsync(AdFilterRequest filter);
        Task<List<Advertisement>> GetActiveAdvertisementsAsync(string position, int? userDistrictId = null,
            int? userCategoryId = null, string? userGender = null, int? userAge = null);

        // Advertisement Media Management
        Task<bool> AddAdvertisementImageAsync(int adId, string imageUrl);
        Task<bool> AddAdvertisementVideoAsync(int adId, string videoUrl);

        // Advertisement Analytics
        Task<bool> IncrementAdViewAsync(int adId, int? userId, string? ipAddress, string? userAgent, string position);
        Task<bool> IncrementAdClickAsync(int adId, int? userId, string? ipAddress, string? userAgent, string? referrer);
        Task<AdAnalyticsResponse> GetAdvertisementAnalyticsAsync(int adId);

        // Advertisement Inquiries
        Task<int> CreateAdInquiryAsync(AdInquiry inquiry);
        Task<(List<AdInquiry> Inquiries, int TotalCount)> GetAdInquiriesAsync(int? adId = null, string? status = null,
            int pageSize = 20, int pageNumber = 1);
        Task<bool> UpdateInquiryStatusAsync(int inquiryId, string status);
    }
}