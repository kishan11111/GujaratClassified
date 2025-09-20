using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAgriFieldRepository
    {
        Task<int> CreateAgriFieldAsync(AgriField agriField);
        Task<AgriField?> GetAgriFieldByIdAsync(int agriFieldId, int? currentUserId = null);
        Task<(List<AgriField> AgriFields, int TotalCount)> GetAgriFieldsAsync(AgriFieldSearchRequest request, int? currentUserId = null);
        Task<(List<AgriField> AgriFields, int TotalCount)> GetUserAgriFieldsAsync(int userId, int pageNumber = 1, int pageSize = 20, string? status = null);
        Task<bool> UpdateAgriFieldAsync(int agriFieldId, AgriField agriField);
        Task<bool> DeleteAgriFieldAsync(int agriFieldId, int userId);
        Task<bool> UpdateAgriFieldStatusAsync(int agriFieldId, string status);
        Task<bool> IncrementViewCountAsync(int agriFieldId);
        Task<List<AgriField>> GetFeaturedAgriFieldsAsync(int limit = 10);
        Task<List<AgriField>> GetNearbyAgriFieldsAsync(int districtId, int? talukaId = null, int limit = 20);
        Task<List<AgriField>> GetTrendingAgriFieldsAsync(int days = 7, int limit = 20);
    }
}