using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAgriFieldFollowRepository
    {
        Task<bool> FollowAgriFieldAsync(int agriFieldId, int userId);
        Task<bool> UnfollowAgriFieldAsync(int agriFieldId, int userId);
        Task<bool> IsFollowedByUserAsync(int agriFieldId, int userId);
        Task<List<AgriFieldFollow>> GetFollowersAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50);
        Task<int> GetFollowerCountAsync(int agriFieldId);
        Task<bool> UpdateFollowerCountAsync(int agriFieldId, int increment = 1);
        Task<List<AgriField>> GetFollowedAgriFieldsAsync(int userId, int pageNumber = 1, int pageSize = 20);
    }
}