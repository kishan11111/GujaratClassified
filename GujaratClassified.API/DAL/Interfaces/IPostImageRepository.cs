using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IPostImageRepository
    {
        Task<int> AddPostImageAsync(PostImage image);
        Task<List<PostImage>> GetPostImagesAsync(int postId);
        Task<bool> DeletePostImageAsync(int imageId, int postId);
        Task<bool> SetMainImageAsync(int imageId, int postId);
    }
}
