using System.Collections.Generic;
using System.Threading.Tasks;
using GujaratClassified.API.Models.Common;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface ISponsorService
    {
        Task<ApiResponse<SponsorBannerResponse>> GetAppOpenBannerAsync(int? userId, string deviceId);
        Task<ApiResponse<List<SponsorCardResponse>>> GetSponsorCardsAsync(int count = 5);
        Task<ApiResponse<bool>> TrackSponsorClickAsync(int sponsorId, int? userId, string deviceId);
        Task<ApiResponse<SystemParameterResponse>> GetSystemParameterAsync(string parameterName);
        Task<ApiResponse<Dictionary<string, string>>> GetAllSystemParametersAsync();
    }
}
