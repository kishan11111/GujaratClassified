using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IPostCommentRepository
    {
        Task<int> AddCommentAsync(PostComment comment);
        Task<PostComment?> GetCommentByIdAsync(int commentId);
        Task<bool> UpdateCommentAsync(int commentId, int userId, string commentText);
        Task<bool> DeleteCommentAsync(int commentId, int userId);
        Task<(List<PostComment> Comments, int TotalCount)> GetPostCommentsAsync(int postId, int pageSize, int pageNumber, string sortBy, bool includeReplies);
        Task<List<PostComment>> GetCommentRepliesAsync(int parentCommentId);
        Task<int> GetPostCommentCountAsync(int postId);
        Task<bool> CanUserModifyCommentAsync(int commentId, int userId);
    }
}