using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IBlogRepository
    {
        Task<(List<Blog> Blogs, int TotalCount)> GetAllBlogsAsync(BlogFilterRequest filter);
        Task<Blog?> GetBlogByIdAsync(int blogId);
        Task<List<Blog>> GetFeaturedBlogsAsync(int count);
        Task<List<Blog>> GetRelatedBlogsAsync(int blogId, int count);
        Task<bool> IncrementViewCountAsync(int blogId);
    }
}