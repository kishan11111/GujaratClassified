using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/post/like")]
    [Authorize]
    [Produces("application/json")]
    public class PostLikeController : ControllerBase
    {
        private readonly IPostLikeService _likeService;

        public PostLikeController(IPostLikeService likeService)
        {
            _likeService = likeService;
        }

        /// <summary>
        /// Toggle like/unlike on a post
        /// </summary>
        /// <param name="request">Post ID to toggle like</param>
        /// <returns>Like status and total count</returns>
        [HttpPost("toggle")]
        public async Task<IActionResult> ToggleLike([FromBody] LikeToggleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _likeService.ToggleLikeAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get all likes for a specific post
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="pageSize">Number of likes per page (default: 20)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <returns>List of users who liked the post</returns>
        [HttpGet("post/{postId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPostLikes(
            int postId,
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            if (pageSize <= 0 || pageSize > 100)
                pageSize = 20;

            if (pageNumber <= 0)
                pageNumber = 1;

            var result = await _likeService.GetPostLikesAsync(postId, pageSize, pageNumber);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get all posts liked by the current user
        /// </summary>
        /// <param name="pageSize">Number of likes per page (default: 20)</param>
        /// <param name="pageNumber">Page number (default: 1)</param>
        /// <returns>List of posts liked by the user</returns>
        [HttpGet("my-likes")]
        public async Task<IActionResult> GetMyLikes(
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            if (pageSize <= 0 || pageSize > 100)
                pageSize = 20;

            if (pageNumber <= 0)
                pageNumber = 1;

            var userId = GetCurrentUserId();
            var result = await _likeService.GetUserLikesAsync(userId, pageSize, pageNumber);

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
    }
}
