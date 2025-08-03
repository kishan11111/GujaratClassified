using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IUserPostService
    {
        Task<ApiResponse<PostResponse>> CreatePostAsync(int userId, CreatePostRequest request);
        Task<ApiResponse<PostResponse>> GetPostByIdAsync(int postId, int? viewerUserId = null);
        Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetMyPostsAsync(int userId, string? status, int pageSize, int pageNumber);
        Task<ApiResponse<PostResponse>> UpdatePostAsync(int postId, int userId, UpdatePostRequest request);
        Task<ApiResponse<object>> UpdatePostStatusAsync(int postId, int userId, PostStatusRequest request);
        Task<ApiResponse<object>> DeletePostAsync(int postId, int userId);
        Task<ApiResponse<List<UploadResponse>>> UploadPostImagesAsync(int postId, int userId, List<IFormFile> images);
        Task<ApiResponse<UploadResponse>> UploadPostVideoAsync(int postId, int userId, IFormFile video);
        Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetUserFavoritesAsync(int userId, int pageSize, int pageNumber);
        Task<ApiResponse<string>> ToggleFavoriteAsync(int userId, int postId);
        Task<ApiResponse<PostStatsResponse>> GetUserPostStatsAsync(int userId);
        Task<ApiResponse<object>> ContactSellerAsync(int userId, ContactSellerRequest request);
    }
}
