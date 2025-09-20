using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/post/comment")]
    [Produces("application/json")]
    public class PostCommentController : ControllerBase
    {
        private readonly IPostCommentService _commentService;

        public PostCommentController(IPostCommentService commentService)
        {
            _commentService = commentService;
        }

        /// <summary>
        /// Add a new comment to a post
        /// </summary>
        /// <param name="request">Comment details</param>
        /// <returns>Created comment details</returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment([FromBody] AddCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _commentService.AddCommentAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Update an existing comment
        /// </summary>
        /// <param name="commentId">Comment ID to update</param>
        /// <param name="request">Updated comment text</param>
        /// <returns>Updated comment details</returns>
        [HttpPut("{commentId}")]
        [Authorize]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _commentService.UpdateCommentAsync(userId, commentId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <param name="commentId">Comment ID to delete</param>
        /// <returns>Success confirmation</returns>
        [HttpDelete("{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = GetCurrentUserId();
            var result = await _commentService.DeleteCommentAsync(userId, commentId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get all comments for a post
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="pageSize">Number of comments per page (default: 10)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <param name="sortBy">Sort order: NEWEST or OLDEST (default: NEWEST)</param>
        /// <param name="includeReplies">Include replies to comments (default: true)</param>
        /// <returns>List of comments with pagination</returns>
        [HttpGet("post/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostComments(
            int postId,
            [FromQuery] int pageSize = 10,
            [FromQuery] int pageNumber = 1,
            [FromQuery] string sortBy = "NEWEST",
            [FromQuery] bool includeReplies = true)
        {
            var request = new GetCommentsRequest
            {
                PostId = postId,
                PageSize = Math.Max(1, Math.Min(100, pageSize)),
                PageNumber = Math.Max(1, pageNumber),
                SortBy = sortBy,
                IncludeReplies = includeReplies
            };

            if (!TryValidateModel(request))
            {
                return BadRequest(ModelState);
            }

            var currentUserId = GetCurrentUserIdOrNull();
            var result = await _commentService.GetPostCommentsAsync(request, currentUserId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get replies to a specific comment
        /// </summary>
        /// <param name="commentId">Parent comment ID</param>
        /// <returns>List of replies</returns>
        [HttpGet("{commentId}/replies")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentReplies(int commentId)
        {
            var currentUserId = GetCurrentUserIdOrNull();
            var result = await _commentService.GetCommentRepliesAsync(commentId, currentUserId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                             User.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token");
            }

            return userId;
        }

        private int? GetCurrentUserIdOrNull()
        {
            if (!User.Identity?.IsAuthenticated ?? false)
                return null;

            try
            {
                return GetCurrentUserId();
            }
            catch
            {
                return null;
            }
        }
    }
}