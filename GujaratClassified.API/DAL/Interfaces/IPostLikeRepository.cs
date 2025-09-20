using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IPostLikeRepository
    {
        Task<bool> ToggleLikeAsync(int postId, int userId);
        Task<bool> IsPostLikedByUserAsync(int postId, int userId);
        Task<int> GetPostLikeCountAsync(int postId);
        Task<List<PostLike>> GetPostLikesAsync(int postId, int pageSize, int pageNumber);
        Task<List<PostLike>> GetUserLikesAsync(int userId, int pageSize, int pageNumber);
    }
}