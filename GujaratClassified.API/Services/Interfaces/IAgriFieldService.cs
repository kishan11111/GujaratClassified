using GujaratClassified.API.Models.Common;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IAgriFieldService
    {
        Task<ApiResponse<AgriFieldResponse>> CreateAgriFieldAsync(int userId, CreateAgriFieldRequest request);
        Task<ApiResponse<AgriFieldResponse>> GetAgriFieldByIdAsync(int agriFieldId, int? currentUserId = null);
        Task<ApiResponse<PagedResponse<AgriFieldResponse>>> GetAgriFieldsAsync(AgriFieldSearchRequest request, int? currentUserId = null);
        Task<ApiResponse<PagedResponse<AgriFieldResponse>>> GetUserAgriFieldsAsync(int userId, int pageNumber = 1, int pageSize = 20, string? status = null);
        Task<ApiResponse<AgriFieldResponse>> UpdateAgriFieldAsync(int agriFieldId, int userId, UpdateAgriFieldRequest request);
        Task<ApiResponse<bool>> DeleteAgriFieldAsync(int agriFieldId, int userId);
        Task<ApiResponse<bool>> LikeAgriFieldAsync(int agriFieldId, int userId, string? reactionType = "LIKE");
        Task<ApiResponse<bool>> UnlikeAgriFieldAsync(int agriFieldId, int userId);
        Task<ApiResponse<bool>> FollowAgriFieldAsync(int agriFieldId, int userId);
        Task<ApiResponse<bool>> UnfollowAgriFieldAsync(int agriFieldId, int userId);
        Task<ApiResponse<int>> AddCommentAsync(int agriFieldId, int userId, CreateAgriFieldCommentRequest request);
        Task<ApiResponse<List<AgriFieldCommentResponse>>> GetCommentsAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50);
        Task<ApiResponse<bool>> UpdateCommentAsync(int commentId, int userId, string commentText);
        Task<ApiResponse<bool>> DeleteCommentAsync(int commentId, int userId);
        Task<ApiResponse<AgriFieldStatsResponse>> GetDashboardStatsAsync();
        Task<ApiResponse<List<AgriFieldResponse>>> GetFeaturedAgriFieldsAsync(int limit = 10);
        Task<ApiResponse<List<AgriFieldResponse>>> GetNearbyAgriFieldsAsync(int districtId, int? talukaId = null, int limit = 20);
        Task<ApiResponse<List<AgriFieldResponse>>> GetTrendingAgriFieldsAsync(int days = 7, int limit = 20);
        Task<ApiResponse<List<AgriFieldResponse>>> GetFollowedAgriFieldsAsync(int userId, int pageNumber = 1, int pageSize = 20);
    }
}