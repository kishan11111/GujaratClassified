using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Services.Implementations
{
    public class PostCommentService : IPostCommentService
    {
        private readonly IPostCommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly ILogger<PostCommentService> _logger;

        public PostCommentService(
            IPostCommentRepository commentRepository,
            IPostRepository postRepository,
            ILogger<PostCommentService> logger)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<CommentResponse>> AddCommentAsync(int userId, AddCommentRequest request)
        {
            try
            {
                // Check if post exists and is active
                var post = await _postRepository.GetPostByIdAsync(request.PostId);
                if (post == null)
                {
                    return ApiResponse<CommentResponse>.ErrorResponse("Post not found");
                }

                if (!post.IsActive || post.Status != "ACTIVE")
                {
                    return ApiResponse<CommentResponse>.ErrorResponse("Post is not available for comments");
                }

                // If replying to a comment, check if parent comment exists
                if (request.ParentCommentId.HasValue)
                {
                    var parentComment = await _commentRepository.GetCommentByIdAsync(request.ParentCommentId.Value);
                    if (parentComment == null || parentComment.PostId != request.PostId)
                    {
                        return ApiResponse<CommentResponse>.ErrorResponse("Parent comment not found or invalid");
                    }
                }

                // Create comment
                var comment = new PostComment
                {
                    PostId = request.PostId,
                    UserId = userId,
                    ParentCommentId = request.ParentCommentId,
                    CommentText = request.CommentText.Trim(),
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var commentId = await _commentRepository.AddCommentAsync(comment);
                comment.CommentId = commentId;

                // Get the created comment with user details
                var createdComment = await _commentRepository.GetCommentByIdAsync(commentId);
                var response = MapCommentToResponse(createdComment!, userId);

                return ApiResponse<CommentResponse>.SuccessResponse(response, "Comment added successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment for post {PostId} by user {UserId}", request.PostId, userId);
                return ApiResponse<CommentResponse>.ErrorResponse("An error occurred while adding the comment");
            }
        }

        public async Task<ApiResponse<CommentResponse>> UpdateCommentAsync(int userId, int commentId, UpdateCommentRequest request)
        {
            try
            {
                // Check if user can modify this comment
                if (!await _commentRepository.CanUserModifyCommentAsync(commentId, userId))
                {
                    return ApiResponse<CommentResponse>.ErrorResponse("You are not authorized to update this comment");
                }

                // Update comment
                var updated = await _commentRepository.UpdateCommentAsync(commentId, userId, request.CommentText.Trim());
                if (!updated)
                {
                    return ApiResponse<CommentResponse>.ErrorResponse("Failed to update comment");
                }

                // Get updated comment
                var updatedComment = await _commentRepository.GetCommentByIdAsync(commentId);
                var response = MapCommentToResponse(updatedComment!, userId);

                return ApiResponse<CommentResponse>.SuccessResponse(response, "Comment updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment {CommentId} by user {UserId}", commentId, userId);
                return ApiResponse<CommentResponse>.ErrorResponse("An error occurred while updating the comment");
            }
        }

        public async Task<ApiResponse<object>> DeleteCommentAsync(int userId, int commentId)
        {
            try
            {
                // Check if user can modify this comment
                if (!await _commentRepository.CanUserModifyCommentAsync(commentId, userId))
                {
                    return ApiResponse<object>.ErrorResponse("You are not authorized to delete this comment");
                }

                // Delete comment
                var deleted = await _commentRepository.DeleteCommentAsync(commentId, userId);
                if (!deleted)
                {
                    return ApiResponse<object>.ErrorResponse("Failed to delete comment");
                }

                return ApiResponse<object>.SuccessResponse(null, "Comment deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment {CommentId} by user {UserId}", commentId, userId);
                return ApiResponse<object>.ErrorResponse("An error occurred while deleting the comment");
            }
        }

        public async Task<ApiResponse<CommentsListResponse>> GetPostCommentsAsync(GetCommentsRequest request, int? currentUserId = null)
        {
            try
            {
                var (comments, totalCount) = await _commentRepository.GetPostCommentsAsync(
                    request.PostId,
                    request.PageSize,
                    request.PageNumber,
                    request.SortBy ?? "NEWEST",
                    request.IncludeReplies);

                var commentResponses = comments.Select(c => MapCommentToResponse(c, currentUserId)).ToList();

                // If including replies, load them
                if (request.IncludeReplies)
                {
                    foreach (var comment in commentResponses.Where(c => c.ParentCommentId == null))
                    {
                        var replies = await _commentRepository.GetCommentRepliesAsync(comment.CommentId);
                        comment.Replies = replies.Select(r => MapCommentToResponse(r, currentUserId)).ToList();
                        comment.ReplyCount = comment.Replies.Count;
                    }
                }

                var pagination = new PaginationResponse
                {
                    CurrentPage = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalRecords = totalCount, // fixed property name
                    TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
                    // No need to set HasNext/HasPrevious, they are auto-computed
                };


                var response = new CommentsListResponse
                {
                    Comments = commentResponses,
                    TotalComments = totalCount,
                    Pagination = pagination
                };

                return ApiResponse<CommentsListResponse>.SuccessResponse(response, "Comments retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting comments for post {PostId}", request.PostId);
                return ApiResponse<CommentsListResponse>.ErrorResponse("An error occurred while retrieving comments");
            }
        }

        public async Task<ApiResponse<List<CommentResponse>>> GetCommentRepliesAsync(int commentId, int? currentUserId = null)
        {
            try
            {
                var replies = await _commentRepository.GetCommentRepliesAsync(commentId);
                var response = replies.Select(r => MapCommentToResponse(r, currentUserId)).ToList();

                return ApiResponse<List<CommentResponse>>.SuccessResponse(response, "Replies retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting replies for comment {CommentId}", commentId);
                return ApiResponse<List<CommentResponse>>.ErrorResponse("An error occurred while retrieving replies");
            }
        }

        private CommentResponse MapCommentToResponse(PostComment comment, int? currentUserId)
        {
            return new CommentResponse
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                UserId = comment.UserId,
                ParentCommentId = comment.ParentCommentId,
                CommentText = comment.CommentText,
                UserName = comment.UserName ?? "Anonymous",
                UserVerified = comment.UserVerified ?? false,
                ReplyCount = comment.ReplyCount,
                CreatedAt = comment.CreatedAt,
                UpdatedAt = comment.UpdatedAt,
                CanEdit = currentUserId.HasValue && currentUserId.Value == comment.UserId,
                CanDelete = currentUserId.HasValue && currentUserId.Value == comment.UserId
            };
        }
    }
}