using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAgriFieldCommentRepository
    {
        Task<int> AddCommentAsync(AgriFieldComment comment);
        Task<List<AgriFieldComment>> GetCommentsAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50);
        Task<bool> UpdateCommentAsync(int commentId, int userId, string commentText);
        Task<bool> DeleteCommentAsync(int commentId, int userId);
        Task<AgriFieldComment?> GetCommentByIdAsync(int commentId);
        Task<bool> IncrementCommentCountAsync(int agriFieldId);
        Task<bool> DecrementCommentCountAsync(int agriFieldId);
    }
}