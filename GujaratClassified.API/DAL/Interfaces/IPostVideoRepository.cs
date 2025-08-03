using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IPostVideoRepository
    {
        Task<int> AddPostVideoAsync(PostVideo video);
        Task<List<PostVideo>> GetPostVideosAsync(int postId);
        Task<bool> DeletePostVideoAsync(int videoId, int postId);
    }
}
