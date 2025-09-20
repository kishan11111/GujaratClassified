using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IPostCommentService
    {
        Task<ApiResponse<CommentResponse>> AddCommentAsync(int userId, AddCommentRequest request);
        Task<ApiResponse<CommentResponse>> UpdateCommentAsync(int userId, int commentId, UpdateCommentRequest request);
        Task<ApiResponse<object>> DeleteCommentAsync(int userId, int commentId);
        Task<ApiResponse<CommentsListResponse>> GetPostCommentsAsync(GetCommentsRequest request, int? currentUserId = null);
        Task<ApiResponse<List<CommentResponse>>> GetCommentRepliesAsync(int commentId, int? currentUserId = null);
    }
}