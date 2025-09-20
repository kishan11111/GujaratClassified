// Controllers/AgriFieldController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/agrifield")]
    [Produces("application/json")]
    public class AgriFieldController : ControllerBase
    {
        private readonly IAgriFieldService _agriFieldService;

        public AgriFieldController(IAgriFieldService agriFieldService)
        {
            _agriFieldService = agriFieldService;
        }

        /// <summary>
        /// Create a new farm post
        /// </summary>
        /// <param name="request">Farm post creation data</param>
        /// <returns>Created farm post details</returns>
        [HttpPost("create")]
        //[Authorize]
        public async Task<IActionResult> CreateAgriField([FromBody] CreateAgriFieldRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _agriFieldService.CreateAgriFieldAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get farm post by ID
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <returns>Farm post details</returns>
        //[HttpGet("{id}")]
        //public async Task<IActionResult> GetAgriField(int id)
        //{
        //    var currentUserId = GetCurrentUserIdOptional();
        //    var result = "";

        //    if (result.Success)
        //    {
        //        return Ok(result);
        //    }

        //    return BadRequest(result);
        //}

        /// <summary>
        /// Get current user's farm posts
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="status">Filter by status</param>
        /// <returns>User's farm posts</returns>
        [HttpGet("my-posts")]
        //[Authorize]
        public async Task<IActionResult> GetMyAgriFields(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? status = null)
        {
            var userId = GetCurrentUserId();
            var result = await _agriFieldService.GetUserAgriFieldsAsync(userId, pageNumber, pageSize, status);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Update farm post
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <param name="request">Update data</param>
        /// <returns>Updated farm post</returns>
        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateAgriField(int id, [FromBody] UpdateAgriFieldRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _agriFieldService.UpdateAgriFieldAsync(id, userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Delete farm post
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteAgriField(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _agriFieldService.DeleteAgriFieldAsync(id, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Like a farm post
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <param name="reactionType">Type of reaction (LIKE, LOVE, etc.)</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/like")]
        //[Authorize]
        public async Task<IActionResult> LikeAgriField(int id, [FromBody] string? reactionType = "LIKE")
        {
            var userId = GetCurrentUserId();
            var result = await _agriFieldService.LikeAgriFieldAsync(id, userId, reactionType);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Unlike a farm post
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}/like")]
        //[Authorize]
        public async Task<IActionResult> UnlikeAgriField(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _agriFieldService.UnlikeAgriFieldAsync(id, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Follow a farm
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/follow")]
        //[Authorize]
        public async Task<IActionResult> FollowAgriField(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _agriFieldService.FollowAgriFieldAsync(id, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Unfollow a farm
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}/follow")]
        //[Authorize]
        public async Task<IActionResult> UnfollowAgriField(int id)
        {
            var userId = GetCurrentUserId();
            var result = await _agriFieldService.UnfollowAgriFieldAsync(id, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Add comment to farm post
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <param name="request">Comment data</param>
        /// <returns>Created comment ID</returns>
        [HttpPost("{id}/comments")]
        //[Authorize]
        public async Task<IActionResult> AddComment(int id, [FromBody] CreateAgriFieldCommentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _agriFieldService.AddCommentAsync(id, userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get comments for farm post
        /// </summary>
        /// <param name="id">Farm post ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of comments</returns>
        [HttpGet("{id}/comments")]
        public async Task<IActionResult> GetComments(int id, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
        {
            var result = await _agriFieldService.GetCommentsAsync(id, pageNumber, pageSize);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Update comment
        /// </summary>
        /// <param name="commentId">Comment ID</param>
        /// <param name="commentText">New comment text</param>
        /// <returns>Success status</returns>
        [HttpPut("comments/{commentId}")]
        //[Authorize]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                return BadRequest("Comment text is required");
            }

            var userId = GetCurrentUserId();
            var result = await _agriFieldService.UpdateCommentAsync(commentId, userId, commentText);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Delete comment
        /// </summary>
        /// <param name="commentId">Comment ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("comments/{commentId}")]
        //[Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var userId = GetCurrentUserId();
            var result = await _agriFieldService.DeleteCommentAsync(commentId, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get AgriField dashboard statistics
        /// </summary>
        /// <returns>Dashboard stats</returns>
        [HttpGet("stats/dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var result = await _agriFieldService.GetDashboardStatsAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get featured farm posts
        /// </summary>
        /// <param name="limit">Number of posts to return</param>
        /// <returns>List of featured farm posts</returns>
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedAgriFields([FromQuery] int limit = 10)
        {
            var result = await _agriFieldService.GetFeaturedAgriFieldsAsync(limit);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get nearby farm posts
        /// </summary>
        /// <param name="districtId">District ID</param>
        /// <param name="talukaId">Taluka ID (optional)</param>
        /// <param name="limit">Number of posts to return</param>
        /// <returns>List of nearby farm posts</returns>
        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyAgriFields(
            [FromQuery] int districtId,
            [FromQuery] int? talukaId = null,
            [FromQuery] int limit = 20)
        {
            var result = await _agriFieldService.GetNearbyAgriFieldsAsync(districtId, talukaId, limit);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get trending farm posts
        /// </summary>
        /// <param name="days">Number of days to consider for trending</param>
        /// <param name="limit">Number of posts to return</param>
        /// <returns>List of trending farm posts</returns>
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrendingAgriFields([FromQuery] int days = 7, [FromQuery] int limit = 20)
        {
            var result = await _agriFieldService.GetTrendingAgriFieldsAsync(days, limit);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get followed farm posts for current user
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of followed farm posts</returns>
        [HttpGet("following")]
        //[Authorize]
        public async Task<IActionResult> GetFollowedAgriFields(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var userId = GetCurrentUserId();
            var result = await _agriFieldService.GetFollowedAgriFieldsAsync(userId, pageNumber, pageSize);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        #region Private Helper Methods

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("User ID not found in token");
        }

        private int? GetCurrentUserIdOptional()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }

        #endregion
    }
}
