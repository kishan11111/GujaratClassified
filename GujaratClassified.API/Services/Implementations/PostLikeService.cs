using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Services.Implementations
{
    public class PostLikeService : IPostLikeService
    {
        private readonly IPostLikeRepository _likeRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PostLikeService> _logger;

        public PostLikeService(
            IPostLikeRepository likeRepository,
            IPostRepository postRepository,
            ILogger<PostLikeService> logger)
        {
            _likeRepository = likeRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<LikeResponse>> ToggleLikeAsync(int userId, LikeToggleRequest request)
        {
            try
            {
                // Check if post exists
                var post = await _postRepository.GetPostByIdAsync(request.PostId);
                if (post == null)
                {
                    return ApiResponse<LikeResponse>.ErrorResponse("Post not found");
                }

                // Check if post is active
                if (!post.IsActive || post.Status != "ACTIVE")
                {
                    return ApiResponse<LikeResponse>.ErrorResponse("Post is not available for likes");
                }

                // Toggle like
                var isLiked = await _likeRepository.ToggleLikeAsync(request.PostId, userId);
                var totalLikes = await _likeRepository.GetPostLikeCountAsync(request.PostId);

                var response = new LikeResponse
                {
                    IsLiked = isLiked,
                    TotalLikes = totalLikes,
                    Message = isLiked ? "Post liked successfully" : "Post unliked successfully"
                };

                return ApiResponse<LikeResponse>.SuccessResponse(response, response.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling like for post {PostId} by user {UserId}", request.PostId, userId);
                return ApiResponse<LikeResponse>.ErrorResponse("An error occurred while processing your request");
            }
        }

        public async Task<ApiResponse<List<PostLike>>> GetPostLikesAsync(int postId, int pageSize, int pageNumber)
        {
            try
            {
                var likes = await _likeRepository.GetPostLikesAsync(postId, pageSize, pageNumber);
                return ApiResponse<List<PostLike>>.SuccessResponse(likes, "Likes retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting likes for post {PostId}", postId);
                return ApiResponse<List<PostLike>>.ErrorResponse("An error occurred while retrieving likes");
            }
        }

        public async Task<ApiResponse<List<PostLike>>> GetUserLikesAsync(int userId, int pageSize, int pageNumber)
        {
            try
            {
                var likes = await _likeRepository.GetUserLikesAsync(userId, pageSize, pageNumber);
                return ApiResponse<List<PostLike>>.SuccessResponse(likes, "User likes retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting likes for user {UserId}", userId);
                return ApiResponse<List<PostLike>>.ErrorResponse("An error occurred while retrieving user likes");
            }
        }
    }
}