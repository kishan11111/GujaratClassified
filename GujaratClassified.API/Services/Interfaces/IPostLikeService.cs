using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IPostLikeService
    {
        Task<ApiResponse<LikeResponse>> ToggleLikeAsync(int userId, LikeToggleRequest request);
        Task<ApiResponse<List<PostLike>>> GetPostLikesAsync(int postId, int pageSize, int pageNumber);
        Task<ApiResponse<List<PostLike>>> GetUserLikesAsync(int userId, int pageSize, int pageNumber);
    }
}