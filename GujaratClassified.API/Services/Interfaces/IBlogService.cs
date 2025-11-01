using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IBlogService
    {
        Task<ApiResponse<BlogListWithPaginationResponse>> GetAllBlogsAsync(BlogFilterRequest filter);
        Task<ApiResponse<BlogDetailResponse>> GetBlogByIdAsync(int blogId);
        Task<ApiResponse<List<BlogListResponse>>> GetFeaturedBlogsAsync(int count = 5);
    }
}