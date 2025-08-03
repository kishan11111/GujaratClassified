// Controllers/UserPostController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/user/post")]
    [Produces("application/json")]
    [Authorize] // All post management requires authentication
    public class UserPostController : ControllerBase
    {
        private readonly IUserPostService _userPostService;

        public UserPostController(IUserPostService userPostService)
        {
            _userPostService = userPostService;
        }

        /// <summary>
        /// Create a new post/listing
        /// </summary>
        /// <param name="request">Post creation data</param>
        /// <returns>Created post details</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _userPostService.CreatePostAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get user's own posts
        /// </summary>
        /// <param name="status">Filter by status (ACTIVE, SOLD, EXPIRED, etc.)</param>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>List of user's posts</returns>
        [HttpGet("my-posts")]
        public async Task<IActionResult> GetMyPosts(
            [FromQuery] string? status = null,
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            var userId = GetCurrentUserId();
            var result = await _userPostService.GetMyPostsAsync(userId, status, pageSize, pageNumber);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get post details by ID
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <returns>Post details with images and videos</returns>
        [HttpGet("{postId}")]
        [AllowAnonymous] // Allow anonymous users to view posts
        public async Task<IActionResult> GetPostById(int postId)
        {
            var userId = GetCurrentUserIdOrNull();
            var result = await _userPostService.GetPostByIdAsync(postId, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Update an existing post
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="request">Post update data</param>
        /// <returns>Updated post details</returns>
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdatePost(int postId, [FromBody] UpdatePostRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _userPostService.UpdatePostAsync(postId, userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Update post status (mark as sold, expired, etc.)
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="request">Status update data</param>
        /// <returns>Success message</returns>
        [HttpPut("{postId}/status")]
        public async Task<IActionResult> UpdatePostStatus(int postId, [FromBody] PostStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _userPostService.UpdatePostStatusAsync(postId, userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Delete a post
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var userId = GetCurrentUserId();
            var result = await _userPostService.DeletePostAsync(postId, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Upload images for a post
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="images">Image files to upload</param>
        /// <returns>Upload results for each image</returns>
        [HttpPost("{postId}/upload-images")]
        public async Task<IActionResult> UploadPostImages(int postId, [FromForm] List<IFormFile> images)
        {
            if (images == null || images.Count == 0)
            {
                return BadRequest(new { success = false, message = "No images provided" });
            }

            if (images.Count > 10)
            {
                return BadRequest(new { success = false, message = "Maximum 10 images allowed per post" });
            }

            var userId = GetCurrentUserId();
            var result = await _userPostService.UploadPostImagesAsync(postId, userId, images);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Upload video for a post
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <param name="video">Video file to upload</param>
        /// <returns>Upload result</returns>
        [HttpPost("{postId}/upload-video")]
        public async Task<IActionResult> UploadPostVideo(int postId, [FromForm] IFormFile video)
        {
            if (video == null)
            {
                return BadRequest(new { success = false, message = "No video provided" });
            }

            var userId = GetCurrentUserId();
            var result = await _userPostService.UploadPostVideoAsync(postId, userId, video);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get user's favorite posts
        /// </summary>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>List of favorite posts</returns>
        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavorites(
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            var userId = GetCurrentUserId();
            var result = await _userPostService.GetUserFavoritesAsync(userId, pageSize, pageNumber);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Add or remove post from favorites
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <returns>Action taken (ADDED or REMOVED)</returns>
        [HttpPost("favorite/{postId}")]
        public async Task<IActionResult> ToggleFavorite(int postId)
        {
            var userId = GetCurrentUserId();
            var result = await _userPostService.ToggleFavoriteAsync(userId, postId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get user's post statistics
        /// </summary>
        /// <returns>Post statistics (total, active, sold, views, etc.)</returns>
        [HttpGet("stats")]
        public async Task<IActionResult> GetPostStats()
        {
            var userId = GetCurrentUserId();
            var result = await _userPostService.GetUserPostStatsAsync(userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Contact seller for a post
        /// </summary>
        /// <param name="request">Contact seller data</param>
        /// <returns>Success message</returns>
        [HttpPost("contact-seller")]
        public async Task<IActionResult> ContactSeller([FromBody] ContactSellerRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetCurrentUserId();
            var result = await _userPostService.ContactSellerAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }

        private int? GetCurrentUserIdOrNull()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}