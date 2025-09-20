using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAgriFieldLikeRepository
    {
        Task<bool> LikeAgriFieldAsync(int agriFieldId, int userId, string? reactionType = "LIKE");
        Task<bool> UnlikeAgriFieldAsync(int agriFieldId, int userId);
        Task<bool> IsLikedByUserAsync(int agriFieldId, int userId);
        Task<List<AgriFieldLike>> GetAgriFieldLikesAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50);
        Task<int> GetLikeCountAsync(int agriFieldId);
        Task<bool> UpdateLikeCountAsync(int agriFieldId, int increment = 1);
    }
}