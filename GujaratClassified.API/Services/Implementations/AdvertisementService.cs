// Services/Implementations/AdvertisementService.cs
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.Services.Implementations
{
    public class AdvertisementService : IAdvertisementService
    {
        private readonly IAdvertisementRepository _advertisementRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<AdvertisementService> _logger;

        public AdvertisementService(
            IAdvertisementRepository advertisementRepository,
            IFileUploadService fileUploadService,
            ILogger<AdvertisementService> logger)
        {
            _advertisementRepository = advertisementRepository;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        public async Task<ApiResponse<int>> CreateAdvertisementAsync(CreateAdvertisementRequest request)
        {
            try
            {
                // Validate dates
                if (request.StartDate >= request.EndDate)
                {
                    return ApiResponse<int>.ErrorResponse("End date must be after start date",
                        new List<string> { "Invalid date range" });
                }

                if (request.StartDate < DateTime.UtcNow.Date)
                {
                    return ApiResponse<int>.ErrorResponse("Start date cannot be in the past",
                        new List<string> { "Invalid start date" });
                }

                var advertisement = new Advertisement
                {
                    Title = request.Title,
                    Description = request.Description,
                    AdType = request.AdType,
                    Position = request.Position,
                    TargetUrl = request.TargetUrl,
                    ContactPhone = request.ContactPhone,
                    ContactEmail = request.ContactEmail,
                    CompanyName = request.CompanyName,
                    CompanyAddress = request.CompanyAddress,
                    Price = request.Price,
                    PriceType = request.PriceType,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    Priority = request.Priority,
                    TargetDistrictId = request.TargetDistrictId,
                    TargetTalukaId = request.TargetTalukaId,
                    TargetCategoryId = request.TargetCategoryId,
                    TargetAgeGroup = request.TargetAgeGroup,
                    TargetGender = request.TargetGender
                };

                var adId = await _advertisementRepository.CreateAdvertisementAsync(advertisement);

                return ApiResponse<int>.SuccessResponse(adId, "Advertisement created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating advertisement");
                return ApiResponse<int>.ErrorResponse("An error occurred while creating advertisement",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AdvertisementResponse>> GetAdvertisementByIdAsync(int adId)
        {
            try
            {
                var advertisement = await _advertisementRepository.GetAdvertisementByIdAsync(adId);
                if (advertisement == null)
                {
                    return ApiResponse<AdvertisementResponse>.ErrorResponse("Advertisement not found",
                        new List<string> { "Invalid advertisement ID" });
                }

                var response = MapAdvertisementToResponse(advertisement);
                return ApiResponse<AdvertisementResponse>.SuccessResponse(response, "Advertisement retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting advertisement by ID: {AdId}", adId);
                return ApiResponse<AdvertisementResponse>.ErrorResponse("An error occurred while retrieving advertisement",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<(List<AdListResponse> Ads, PaginationResponse Pagination)>> GetAdvertisementsWithFiltersAsync(AdFilterRequest filter)
        {
            try
            {
                var (ads, totalCount) = await _advertisementRepository.GetAdvertisementsWithFiltersAsync(filter);

                var adResponses = ads.Select(MapAdvertisementToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                return ApiResponse<(List<AdListResponse>, PaginationResponse)>.SuccessResponse(
                    (adResponses, pagination),
                    $"Retrieved {adResponses.Count} advertisements successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting advertisements with filters");
                return ApiResponse<(List<AdListResponse>, PaginationResponse)>.ErrorResponse(
                    "An error occurred while retrieving advertisements", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UpdateAdvertisementAsync(int adId, UpdateAdvertisementRequest request)
        {
            try
            {
                var existingAd = await _advertisementRepository.GetAdvertisementByIdAsync(adId);
                if (existingAd == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Advertisement not found",
                        new List<string> { "Invalid advertisement ID" });
                }

                // Validate dates
                if (request.StartDate >= request.EndDate)
                {
                    return ApiResponse<bool>.ErrorResponse("End date must be after start date",
                        new List<string> { "Invalid date range" });
                }

                // Update advertisement properties
                existingAd.Title = request.Title;
                existingAd.Description = request.Description;
                existingAd.TargetUrl = request.TargetUrl;
                existingAd.ContactPhone = request.ContactPhone;
                existingAd.ContactEmail = request.ContactEmail;
                existingAd.CompanyName = request.CompanyName;
                existingAd.CompanyAddress = request.CompanyAddress;
                existingAd.Price = request.Price;
                existingAd.StartDate = request.StartDate;
                existingAd.EndDate = request.EndDate;
                existingAd.Priority = request.Priority;
                existingAd.TargetDistrictId = request.TargetDistrictId;
                existingAd.TargetTalukaId = request.TargetTalukaId;
                existingAd.TargetCategoryId = request.TargetCategoryId;
                existingAd.TargetAgeGroup = request.TargetAgeGroup;
                existingAd.TargetGender = request.TargetGender;
                existingAd.UpdatedAt = DateTime.UtcNow;

                var success = await _advertisementRepository.UpdateAdvertisementAsync(existingAd);

                if (success)
                {
                    return ApiResponse<bool>.SuccessResponse(true, "Advertisement updated successfully");
                }
                else
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to update advertisement",
                        new List<string> { "Update operation failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating advertisement: {AdId}", adId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while updating advertisement",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UpdateAdvertisementStatusAsync(int adId, AdStatusRequest request)
        {
            try
            {
                var success = await _advertisementRepository.UpdateAdvertisementStatusAsync(adId, request.Status);

                if (success)
                {
                    return ApiResponse<bool>.SuccessResponse(true, $"Advertisement status updated to {request.Status}");
                }
                else
                {
                    return ApiResponse<bool>.ErrorResponse("Advertisement not found or status update failed",
                        new List<string> { "Invalid advertisement ID or status" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating advertisement status: {AdId}", adId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while updating advertisement status",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> DeleteAdvertisementAsync(int adId)
        {
            try
            {
                var success = await _advertisementRepository.DeleteAdvertisementAsync(adId);

                if (success)
                {
                    return ApiResponse<bool>.SuccessResponse(true, "Advertisement deleted successfully");
                }
                else
                {
                    return ApiResponse<bool>.ErrorResponse("Advertisement not found or delete failed",
                        new List<string> { "Invalid advertisement ID" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting advertisement: {AdId}", adId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while deleting advertisement",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UploadAdvertisementImageAsync(int adId, IFormFile imageFile)
        {
            try
            {
                var existingAd = await _advertisementRepository.GetAdvertisementByIdAsync(adId);
                if (existingAd == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Advertisement not found",
                        new List<string> { "Invalid advertisement ID" });
                }

                var uploadResult = await _fileUploadService.UploadImageAsync(imageFile, "advertisements");
                if (!uploadResult.Success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to upload image", uploadResult.Errors ?? new List<string> { uploadResult.Message });
                }

                var success = await _advertisementRepository.AddAdvertisementImageAsync(adId, uploadResult.FileUrl!);

                if (success)
                {
                    return ApiResponse<bool>.SuccessResponse(true, "Advertisement image uploaded successfully");
                }
                else
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to save image URL",
                        new List<string> { "Database update failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading advertisement image: {AdId}", adId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while uploading image",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UploadAdvertisementVideoAsync(int adId, IFormFile videoFile)
        {
            try
            {
                var existingAd = await _advertisementRepository.GetAdvertisementByIdAsync(adId);
                if (existingAd == null)
                {
                    return ApiResponse<bool>.ErrorResponse("Advertisement not found",
                        new List<string> { "Invalid advertisement ID" });
                }

                var uploadResult = await _fileUploadService.UploadVideoAsync(videoFile, "advertisements");
                if (!uploadResult.Success)
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to upload video", uploadResult.Errors ?? new List<string> { uploadResult.Message });
                }

                var success = await _advertisementRepository.AddAdvertisementVideoAsync(adId, uploadResult.FileUrl!);

                if (success)
                {
                    return ApiResponse<bool>.SuccessResponse(true, "Advertisement video uploaded successfully");
                }
                else
                {
                    return ApiResponse<bool>.ErrorResponse("Failed to save video URL",
                        new List<string> { "Database update failed" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading advertisement video: {AdId}", adId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while uploading video",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<AdAnalyticsResponse>> GetAdvertisementAnalyticsAsync(int adId)
        {
            try
            {
                var analytics = await _advertisementRepository.GetAdvertisementAnalyticsAsync(adId);
                if (analytics == null || analytics.AdId == 0)
                {
                    return ApiResponse<AdAnalyticsResponse>.ErrorResponse("Advertisement not found",
                        new List<string> { "Invalid advertisement ID" });
                }

                return ApiResponse<AdAnalyticsResponse>.SuccessResponse(analytics, "Advertisement analytics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting advertisement analytics: {AdId}", adId);
                return ApiResponse<AdAnalyticsResponse>.ErrorResponse("An error occurred while retrieving analytics",
                    new List<string> { ex.Message });
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

        public async Task<ApiResponse<(List<AdInquiryResponse> Inquiries, PaginationResponse Pagination)>> GetAdInquiriesAsync(int? adId = null, string? status = null, int pageSize = 20, int pageNumber = 1)
        {
            try
            {
                var (inquiries, totalCount) = await _advertisementRepository.GetAdInquiriesAsync(adId, status, pageSize, pageNumber);

                var inquiryResponses = inquiries.Select(MapInquiryToResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return ApiResponse<(List<AdInquiryResponse>, PaginationResponse)>.SuccessResponse(
                    (inquiryResponses, pagination),
                    $"Retrieved {inquiryResponses.Count} inquiries successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting ad inquiries");
                return ApiResponse<(List<AdInquiryResponse>, PaginationResponse)>.ErrorResponse(
                    "An error occurred while retrieving inquiries", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<bool>> UpdateInquiryStatusAsync(int inquiryId, string status)
        {
            try
            {
                var success = await _advertisementRepository.UpdateInquiryStatusAsync(inquiryId, status);

                if (success)
                {
                    return ApiResponse<bool>.SuccessResponse(true, $"Inquiry status updated to {status}");
                }
                else
                {
                    return ApiResponse<bool>.ErrorResponse("Inquiry not found or status update failed",
                        new List<string> { "Invalid inquiry ID or status" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inquiry status: {InquiryId}", inquiryId);
                return ApiResponse<bool>.ErrorResponse("An error occurred while updating inquiry status",
                    new List<string> { ex.Message });
            }
        }

        // Private mapping methods
        private AdvertisementResponse MapAdvertisementToResponse(Advertisement ad)
        {
            return new AdvertisementResponse
            {
                AdId = ad.AdId,
                Title = ad.Title,
                Description = ad.Description,
                AdType = ad.AdType,
                Position = ad.Position,
                ImageUrl = ad.ImageUrl,
                VideoUrl = ad.VideoUrl,
                TargetUrl = ad.TargetUrl,
                ContactPhone = ad.ContactPhone,
                ContactEmail = ad.ContactEmail,
                CompanyName = ad.CompanyName,
                CompanyAddress = ad.CompanyAddress,
                Price = ad.Price,
                PriceType = ad.PriceType,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                IsActive = ad.IsActive,
                IsPaid = ad.IsPaid,
                Status = ad.Status,
                Priority = ad.Priority,
                ViewCount = ad.ViewCount,
                ClickCount = ad.ClickCount,
                InquiryCount = ad.InquiryCount,
                CreatedAt = ad.CreatedAt,
                UpdatedAt = ad.UpdatedAt,
                TargetDistrictId = ad.TargetDistrictId,
                TargetTalukaId = ad.TargetTalukaId,
                TargetCategoryId = ad.TargetCategoryId,
                TargetAgeGroup = ad.TargetAgeGroup,
                TargetGender = ad.TargetGender,
                TargetDistrictName = ad.TargetDistrictName,
                TargetTalukaName = ad.TargetTalukaName,
                TargetCategoryName = ad.TargetCategoryName
            };
        }

        private AdListResponse MapAdvertisementToListResponse(Advertisement ad)
        {
            return new AdListResponse
            {
                AdId = ad.AdId,
                Title = ad.Title,
                AdType = ad.AdType,
                Position = ad.Position,
                ImageUrl = ad.ImageUrl,
                CompanyName = ad.CompanyName,
                Price = ad.Price,
                PriceType = ad.PriceType,
                StartDate = ad.StartDate,
                EndDate = ad.EndDate,
                Status = ad.Status,
                Priority = ad.Priority,
                ViewCount = ad.ViewCount,
                ClickCount = ad.ClickCount,
                CreatedAt = ad.CreatedAt
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