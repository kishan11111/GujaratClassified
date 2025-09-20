using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAgriFieldVideoRepository
    {
        Task<int> AddAgriFieldVideoAsync(AgriFieldVideo video);
        Task<List<AgriFieldVideo>> GetAgriFieldVideosAsync(int agriFieldId);
        Task<bool> DeleteAgriFieldVideoAsync(int videoId, int agriFieldId);
        Task<bool> UpdateVideoCaptionAsync(int videoId, string caption);
    }
}