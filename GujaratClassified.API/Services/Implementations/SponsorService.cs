using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Common;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace GujaratClassified.API.Services.Implementations
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ISystemParameterRepository _parameterRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<SponsorService> _logger;
        private static int? _lastSponsorId = null;
        private static DateTime _lastSponsorTime = DateTime.MinValue;
        private static readonly TimeSpan SponsorRotationInterval = TimeSpan.FromMinutes(5);

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ISystemParameterRepository parameterRepository,
            IHttpContextAccessor httpContextAccessor,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _parameterRepository = parameterRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ApiResponse<SponsorBannerResponse>> GetAppOpenBannerAsync(int? userId, string deviceId)
        {
            try
            {
                // Check if sponsor ads are enabled
                var sponsorEnabled = await _parameterRepository.GetParameterByNameAsync("SPONSOR_ADS_ENABLED");
                if (sponsorEnabled == null || sponsorEnabled.ParameterValue?.ToLower() != "true")
                {
                    return new ApiResponse<SponsorBannerResponse>
                    {
                        Success = false,
                        Message = "Sponsor ads are currently disabled",
                        Data = null
                    };
                }

                // Check if we should rotate to a different sponsor
                int? sponsorIdToExclude = null;
                if (DateTime.Now - _lastSponsorTime < SponsorRotationInterval && _lastSponsorId.HasValue)
                {
                    sponsorIdToExclude = _lastSponsorId;
                }

                // Get active sponsor for banner
                var sponsor = await _sponsorRepository.GetActiveSponsorForBannerAsync(sponsorIdToExclude);
                
                if (sponsor == null)
                {
                    return new ApiResponse<SponsorBannerResponse>
                    {
                        Success = false,
                        Message = "No active sponsors available",
                        Data = null
                    };
                }

                // Update last sponsor tracking
                _lastSponsorId = sponsor.SponsorId;
                _lastSponsorTime = DateTime.Now;

                // Track view
                var ipAddress = GetClientIpAddress();
                await _sponsorRepository.TrackSponsorViewAsync(sponsor.SponsorId, userId, deviceId, ipAddress);

                var response = new SponsorBannerResponse
                {
                    SponsorId = sponsor.SponsorId,
                    SponsorName = sponsor.SponsorName,
                    CompanyName = sponsor.CompanyName,
                    BannerImagePath = sponsor.BannerImagePath,
                    ClickUrl = sponsor.ClickUrl,
                    WhatsappNumber = sponsor.WhatsappNumber,
                    DisplayType = "BANNER"
                };

                return new ApiResponse<SponsorBannerResponse>
                {
                    Success = true,
                    Message = "Sponsor banner retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting app open banner");
                return new ApiResponse<SponsorBannerResponse>
                {
                    Success = false,
                    Message = "An error occurred while retrieving sponsor banner",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<List<SponsorCardResponse>>> GetSponsorCardsAsync(int count = 5)
        {
            try
            {
                // Check if sponsor ads are enabled
                var sponsorEnabled = await _parameterRepository.GetParameterByNameAsync("SPONSOR_ADS_ENABLED");
                if (sponsorEnabled == null || sponsorEnabled.ParameterValue?.ToLower() != "true")
                {
                    return new ApiResponse<List<SponsorCardResponse>>
                    {
                        Success = false,
                        Message = "Sponsor ads are currently disabled",
                        Data = new List<SponsorCardResponse>()
                    };
                }

                // Get max cards from parameters
                var maxCardsParam = await _parameterRepository.GetParameterByNameAsync("MAX_SPONSOR_CARDS");
                if (maxCardsParam != null && int.TryParse(maxCardsParam.ParameterValue, out int maxCards))
                {
                    count = Math.Min(count, maxCards);
                }

                var sponsors = await _sponsorRepository.GetActiveSponsorsForCardsAsync(count);
                
                if (sponsors == null || !sponsors.Any())
                {
                    return new ApiResponse<List<SponsorCardResponse>>
                    {
                        Success = true,
                        Message = "No active sponsor cards available",
                        Data = new List<SponsorCardResponse>()
                    };
                }

                var response = sponsors.Select(s => new SponsorCardResponse
                {
                    SponsorId = s.SponsorId,
                    SponsorName = s.SponsorName,
                    CompanyName = s.CompanyName,
                    CardImagePath = s.CardImagePath,
                    ClickUrl = s.ClickUrl,
                    WhatsappNumber = s.WhatsappNumber
                }).ToList();

                return new ApiResponse<List<SponsorCardResponse>>
                {
                    Success = true,
                    Message = $"Retrieved {response.Count} sponsor cards",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sponsor cards");
                return new ApiResponse<List<SponsorCardResponse>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving sponsor cards",
                    Data = new List<SponsorCardResponse>()
                };
            }
        }

        public async Task<ApiResponse<bool>> TrackSponsorClickAsync(int sponsorId, int? userId, string deviceId)
        {
            try
            {
                var ipAddress = GetClientIpAddress();
                var result = await _sponsorRepository.TrackSponsorClickAsync(sponsorId, userId, deviceId, ipAddress);

                return new ApiResponse<bool>
                {
                    Success = result,
                    Message = result ? "Click tracked successfully" : "Failed to track click",
                    Data = result
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking sponsor click");
                return new ApiResponse<bool>
                {
                    Success = false,
                    Message = "An error occurred while tracking click",
                    Data = false
                };
            }
        }

        public async Task<ApiResponse<SystemParameterResponse>> GetSystemParameterAsync(string parameterName)
        {
            try
            {
                var parameter = await _parameterRepository.GetParameterByNameAsync(parameterName);
                
                if (parameter == null)
                {
                    return new ApiResponse<SystemParameterResponse>
                    {
                        Success = false,
                        Message = $"Parameter '{parameterName}' not found",
                        Data = null
                    };
                }

                var response = new SystemParameterResponse
                {
                    ParameterName = parameter.ParameterName,
                    ParameterValue = parameter.ParameterValue,
                    IsActive = parameter.IsActive
                };

                return new ApiResponse<SystemParameterResponse>
                {
                    Success = true,
                    Message = "Parameter retrieved successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting system parameter: {ParameterName}", parameterName);
                return new ApiResponse<SystemParameterResponse>
                {
                    Success = false,
                    Message = "An error occurred while retrieving parameter",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<Dictionary<string, string>>> GetAllSystemParametersAsync()
        {
            try
            {
                var parameters = await _parameterRepository.GetAllActiveParametersAsync();
                
                var response = parameters.ToDictionary(
                    p => p.ParameterName,
                    p => p.ParameterValue
                );

                return new ApiResponse<Dictionary<string, string>>
                {
                    Success = true,
                    Message = $"Retrieved {response.Count} parameters",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all system parameters");
                return new ApiResponse<Dictionary<string, string>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving parameters",
                    Data = new Dictionary<string, string>()
                };
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return "0.0.0.0";

                // Check for forwarded IP
                var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                // Check for real IP
                var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp;
                }

                // Return connection IP
                return context.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
            }
            catch
            {
                return "0.0.0.0";
            }
        }
    }
}
