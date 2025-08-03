using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IPostRepository
    {
        Task<int> CreatePostAsync(Post post);
        Task<Post?> GetPostByIdAsync(int postId);
        Task<(List<Post> Posts, int TotalCount)> GetUserPostsAsync(int userId, string? status, int pageSize, int pageNumber);
        Task<bool> UpdatePostAsync(Post post);
        Task<bool> UpdatePostStatusAsync(int postId, int userId, string status);
        Task<bool> DeletePostAsync(int postId, int userId);
        Task<(List<Post> Posts, int TotalCount)> GetPostsWithFiltersAsync(PostFilterRequest filter);
        Task<bool> IncrementPostViewAsync(int postId, int? userId, string? ipAddress, string? userAgent);
        Task<(List<Post> Posts, int TotalCount)> GetUserFavoritesAsync(int userId, int pageSize, int pageNumber);
        Task<PostStatsResponse> GetUserPostStatsAsync(int userId);
        Task<bool> IncrementContactCountAsync(int postId);
    }
}
