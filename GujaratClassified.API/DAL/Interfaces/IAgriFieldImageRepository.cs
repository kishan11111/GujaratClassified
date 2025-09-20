using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAgriFieldImageRepository
    {
        Task<int> AddAgriFieldImageAsync(AgriFieldImage image);
        Task<List<AgriFieldImage>> GetAgriFieldImagesAsync(int agriFieldId);
        Task<bool> DeleteAgriFieldImageAsync(int imageId, int agriFieldId);
        Task<bool> SetMainImageAsync(int imageId, int agriFieldId);
        Task<bool> UpdateImageCaptionAsync(int imageId, string caption);
    }
}