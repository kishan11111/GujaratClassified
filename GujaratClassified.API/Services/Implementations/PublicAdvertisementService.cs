// Services/Implementations/PublicAdvertisementService.cs
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.Services.Implementations
{
    public class PublicAdvertisementService : IPublicAdvertisementService
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IAdInquiryRepository _inquiryRepository;
        private readonly ILogger<PublicAdvertisementService> _logger;

        public PublicAdvertisementService(
            IAdvertisementRepository advertisementRepository,
            IAdInquiryRepository inquiryRepository,
            ILogger<PublicAdvertisementService> logger)
        {
            _advertisementRepository = advertisementRepository;
            _inquiryRepository = inquiryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<PublicAdResponse>>> GetAdvertisementsByPositionAsync(
            string position, int? userDistrictId = null, int? userCategoryId = null,
            string? userGender = null, int? userAge = null)
        {
            try
            {
                var ads = await _advertisementRepository.GetActiveAdvertisementsAsync(
                    position, userDistrictId, userCategoryId, userGender, userAge);

                var publicAds = ads.Select(MapAdvertisementToPublicResponse).ToList();

                // Track views for all displayed ads
                foreach (var ad in ads)
                {
                    try
                    {
                        await _advertisementRepository.IncrementAdViewAsync(ad.AdId, null, null, null, position);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to track view for ad: {AdId}", ad.AdId);
                    }
                }

                return ApiResponse<List<PublicAdResponse>>.SuccessResponse(
                    publicAds, $"Retrieved {publicAds.Count} advertisements for position {position}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting advertisements by position: {Position}", position);
                return ApiResponse<List<PublicAdResponse>>.ErrorResponse(
                    "An error occurred while retrieving advertisements", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PublicAdResponse>> GetPublicAdvertisementByIdAsync(int adId)
        {
            try
            {
                var advertisement = await _advertisementRepository.GetAdvertisementByIdAsync(adId);
                if (advertisement == null || advertisement.Status != "APPROVED" || !advertisement.IsActive)
                {
                    return ApiResponse<PublicAdResponse>.ErrorResponse("Advertisement not found or not available",
                        new List<string> { "Invalid advertisement ID" });
                }

                // Check if advertisement is within active date range
                var currentDate = DateTime.UtcNow;
                if (currentDate < advertisement.StartDate || currentDate > advertisement.EndDate)
                {
                    return ApiResponse<PublicAdResponse>.ErrorResponse("Advertisement is not currently active",
                        new List<string> { "Advertisement outside active date range" });
                }

                var response = MapAdvertisementToPublicResponse(advertisement);
                return ApiResponse<PublicAdResponse>.SuccessResponse(response, "Advertisement retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public advertisement by ID: {AdId}", adId);
                return ApiResponse<PublicAdResponse>.ErrorResponse("An error occurred while retrieving advertisement",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<int>> CreateAdInquiryAsync(AdInquiryRequest request, int userId)
        {
            try
            {
                // Validate advertisement exists and is active
                var advertisement = await _advertisementRepository.GetAdvertisementByIdAsync(request.AdId);
                if (advertisement == null || advertisement.Status != "APPROVED" || !advertisement.IsActive)
                {
                    return ApiResponse<int>.ErrorResponse("Advertisement not found or not available for inquiries",
                        new List<string> { "Invalid advertisement ID" });
                }

                // Check if advertisement is within active date range
                var currentDate = DateTime.UtcNow;
                if (currentDate < advertisement.StartDate || currentDate > advertisement.EndDate)
                {
                    return ApiResponse<int>.ErrorResponse("Advertisement is not currently active",
                        new List<string> { "Advertisement outside active date range" });
                }

                var inquiry = new AdInquiry
                {
                    AdId = request.AdId,
                    UserId = userId,
                    InquirerName = request.InquirerName,
                    InquirerPhone = request.InquirerPhone,
                    InquirerEmail = request.InquirerEmail,
                    Message = request.Message,
                    Status = "NEW"
                };

                var inquiryId = await _advertisementRepository.CreateAdInquiryAsync(inquiry);

                return ApiResponse<int>.SuccessResponse(inquiryId, "Inquiry submitted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating ad inquiry for ad: {AdId}", request.AdId);
                return ApiResponse<int>.ErrorResponse("An error occurred while submitting inquiry",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<AdInquiryResponse>>> GetUserAdInquiriesAsync(int userId, int pageSize = 20, int pageNumber = 1)
        {
            try
            {
                var inquiries = await _inquiryRepository.GetUserInquiriesAsync(userId, pageSize, pageNumber);

                var inquiryResponses = inquiries.Select(MapInquiryToResponse).ToList();

                return ApiResponse<List<AdInquiryResponse>>.SuccessResponse(
                    inquiryResponses, $"Retrieved {inquiryResponses.Count} user inquiries successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user ad inquiries for user: {UserId}", userId);
                return ApiResponse<List<AdInquiryResponse>>.ErrorResponse(
                    "An error occurred while retrieving user inquiries", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> TrackAdViewAsync(int adId, int? userId, string? ipAddress, string? userAgent, string position)
        {
            try
            {
                var success = await _advertisementRepository.IncrementAdViewAsync(adId, userId, ipAddress, userAgent, position);
                return ApiResponse<bool>.SuccessResponse(success, "Ad view tracked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking ad view: {AdId}", adId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while tracking ad view",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> TrackAdClickAsync(int adId, int? userId, string? ipAddress, string? userAgent, string? referrer)
        {
            try
            {
                var success = await _advertisementRepository.IncrementAdClickAsync(adId, userId, ipAddress, userAgent, referrer);
                return ApiResponse<bool>.SuccessResponse(success, "Ad click tracked successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking ad click: {AdId}", adId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while tracking ad click",
                    new List<string> { ex.Message });
            }
        }

        // Private mapping methods
        private PublicAdResponse MapAdvertisementToPublicResponse(Advertisement ad)
        {
            return new PublicAdResponse
            {
                AdId = ad.AdId,
                Title = ad.Title,
                Description = ad.Description,
                AdType = ad.AdType,
                Position = ad.Position,
                ImageUrl = ad.ImageUrl,
                VideoUrl = ad.VideoUrl,
                TargetUrl = ad.TargetUrl,
                CompanyName = ad.CompanyName,
                Priority = ad.Priority
                // Excluding sensitive data like prices, contacts, analytics
            };
        }

        private AdInquiryResponse MapInquiryToResponse(AdInquiry inquiry)
        {
            return new AdInquiryResponse
            {
                InquiryId = inquiry.InquiryId,
                AdId = inquiry.AdId,
                UserId = inquiry.UserId,
                InquirerName = inquiry.InquirerName,
                InquirerPhone = inquiry.InquirerPhone,
                InquirerEmail = inquiry.InquirerEmail,
                Message = inquiry.Message,
                Status = inquiry.Status,
                CreatedAt = inquiry.CreatedAt,
                RespondedAt = inquiry.RespondedAt,
                AdTitle = inquiry.AdTitle,
                CompanyName = inquiry.CompanyName,
                UserName = inquiry.UserName
            };
        }
    }
}