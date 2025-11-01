using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IBrowseService
    {
        //Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetAllPostsAsync(PostFilterRequest filter);
        //Task<ApiResponse<PostListWithPaginationResponse>> GetAllPostsAsync(PostFilterRequest filter);
        //Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetPostsByCategoryAsync(int categoryId, PostFilterRequest filter);
        //Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> SearchPostsAsync(string searchTerm, PostFilterRequest filter);
        //Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetPostsByLocationAsync(int districtId, PostFilterRequest filter);
        //Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetFeaturedPostsAsync(PostFilterRequest filter);
        //Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetLatestPostsAsync(PostFilterRequest filter);
        //Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetPopularPostsAsync(PostFilterRequest filter);
        //Task<ApiResponse<PostResponse>> GetPublicPostByIdAsync(int postId, int? viewerUserId = null);

        Task<ApiResponse<PostListWithPaginationResponse>> GetAllPostsAsync(PostFilterRequest filter);
        Task<ApiResponse<PostListWithPaginationResponse>> GetPostsByCategoryAsync(int categoryId, PostFilterRequest filter);
        Task<ApiResponse<PostListWithPaginationResponse>> SearchPostsAsync(string searchTerm, PostFilterRequest filter);
        Task<ApiResponse<PostListWithPaginationResponse>> GetPostsByLocationAsync(int districtId, PostFilterRequest filter);
        Task<ApiResponse<PostListWithPaginationResponse>> GetFeaturedPostsAsync(PostFilterRequest filter);
        Task<ApiResponse<PostListWithPaginationResponse>> GetLatestPostsAsync(PostFilterRequest filter);
        Task<ApiResponse<PostListWithPaginationResponse>> GetPopularPostsAsync(PostFilterRequest filter);
        Task<ApiResponse<PostResponse>> GetPublicPostByIdAsync(int postId, int? viewerUserId = null);


    }
}
