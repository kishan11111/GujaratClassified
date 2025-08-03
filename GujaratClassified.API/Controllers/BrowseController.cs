// Controllers/BrowseController.cs
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/browse")]
    [Produces("application/json")]
    public class BrowseController : ControllerBase
    {
        private readonly IBrowseService _browseService;

        public BrowseController(IBrowseService browseService)
        {
            _browseService = browseService;
        }

        /// <summary>
        /// Get all posts with advanced filters - Home Feed
        /// </summary>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="subCategoryId">Filter by subcategory</param>
        /// <param name="districtId">Filter by district</param>
        /// <param name="talukaId">Filter by taluka</param>
        /// <param name="villageId">Filter by village</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <param name="condition">Filter by condition (NEW, LIKE_NEW, GOOD, FAIR, POOR)</param>
        /// <param name="priceType">Filter by price type (FIXED, NEGOTIABLE, ON_CALL)</param>
        /// <param name="isFeatured">Filter featured posts only</param>
        /// <param name="searchTerm">Search in title and description</param>
        /// <param name="sortBy">Sort by (NEWEST, OLDEST, PRICE_LOW, PRICE_HIGH, POPULAR)</param>
        /// <param name="pageSize">Number of posts per page (1-100)</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>Paginated list of posts</returns>
        [HttpGet("posts")]
        public async Task<IActionResult> GetAllPosts(
            [FromQuery] int? categoryId = null,
            [FromQuery] int? subCategoryId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] int? talukaId = null,
            [FromQuery] int? villageId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? condition = null,
            [FromQuery] string? priceType = null,
            [FromQuery] bool? isFeatured = null,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string sortBy = "NEWEST",
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            var filter = new PostFilterRequest
            {
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                DistrictId = districtId,
                TalukaId = talukaId,
                VillageId = villageId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Condition = condition,
                PriceType = priceType,
                IsFeatured = isFeatured,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                PageSize = Math.Min(pageSize, 100), // Limit max page size
                PageNumber = Math.Max(pageNumber, 1) // Ensure minimum page number
            };

            var result = await _browseService.GetAllPostsAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get posts by specific category - Category Browse
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <param name="subCategoryId">Filter by subcategory</param>
        /// <param name="districtId">Filter by district</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <param name="condition">Filter by condition</param>
        /// <param name="priceType">Filter by price type</param>
        /// <param name="sortBy">Sort by</param>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>Paginated list of posts from the category</returns>
        [HttpGet("posts/category/{categoryId}")]
        public async Task<IActionResult> GetPostsByCategory(
            int categoryId,
            [FromQuery] int? subCategoryId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? condition = null,
            [FromQuery] string? priceType = null,
            [FromQuery] string sortBy = "NEWEST",
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            if (categoryId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid category ID" });
            }

            var filter = new PostFilterRequest
            {
                SubCategoryId = subCategoryId,
                DistrictId = districtId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Condition = condition,
                PriceType = priceType,
                SortBy = sortBy,
                PageSize = Math.Min(pageSize, 100),
                PageNumber = Math.Max(pageNumber, 1)
            };

            var result = await _browseService.GetPostsByCategoryAsync(categoryId, filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Search posts with keywords - Search Results
        /// </summary>
        /// <param name="q">Search query/keywords</param>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="districtId">Filter by district</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <param name="condition">Filter by condition</param>
        /// <param name="sortBy">Sort by</param>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>Paginated search results</returns>
        [HttpGet("posts/search")]
        public async Task<IActionResult> SearchPosts(
            [FromQuery] string q,
            [FromQuery] int? categoryId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string? condition = null,
            [FromQuery] string sortBy = "NEWEST",
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return BadRequest(new { success = false, message = "Search query is required" });
            }

            var filter = new PostFilterRequest
            {
                CategoryId = categoryId,
                DistrictId = districtId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Condition = condition,
                SortBy = sortBy,
                PageSize = Math.Min(pageSize, 100),
                PageNumber = Math.Max(pageNumber, 1)
            };

            var result = await _browseService.SearchPostsAsync(q.Trim(), filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get posts by location (district) - Location Filter
        /// </summary>
        /// <param name="districtId">District ID</param>
        /// <param name="talukaId">Filter by taluka</param>
        /// <param name="villageId">Filter by village</param>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="minPrice">Minimum price filter</param>
        /// <param name="maxPrice">Maximum price filter</param>
        /// <param name="sortBy">Sort by</param>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>Paginated list of posts from the location</returns>
        [HttpGet("posts/location/{districtId}")]
        public async Task<IActionResult> GetPostsByLocation(
            int districtId,
            [FromQuery] int? talukaId = null,
            [FromQuery] int? villageId = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] string sortBy = "NEWEST",
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            if (districtId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid district ID" });
            }

            var filter = new PostFilterRequest
            {
                TalukaId = talukaId,
                VillageId = villageId,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                PageSize = Math.Min(pageSize, 100),
                PageNumber = Math.Max(pageNumber, 1)
            };

            var result = await _browseService.GetPostsByLocationAsync(districtId, filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get featured posts - Featured Section
        /// </summary>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="districtId">Filter by district</param>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>Paginated list of featured posts</returns>
        [HttpGet("posts/featured")]
        public async Task<IActionResult> GetFeaturedPosts(
            [FromQuery] int? categoryId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            var filter = new PostFilterRequest
            {
                CategoryId = categoryId,
                DistrictId = districtId,
                PageSize = Math.Min(pageSize, 100),
                PageNumber = Math.Max(pageNumber, 1)
            };

            var result = await _browseService.GetFeaturedPostsAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get latest posts - Latest Section
        /// </summary>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="districtId">Filter by district</param>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>Paginated list of latest posts</returns>
        [HttpGet("posts/latest")]
        public async Task<IActionResult> GetLatestPosts(
            [FromQuery] int? categoryId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            var filter = new PostFilterRequest
            {
                CategoryId = categoryId,
                DistrictId = districtId,
                PageSize = Math.Min(pageSize, 100),
                PageNumber = Math.Max(pageNumber, 1)
            };

            var result = await _browseService.GetLatestPostsAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get popular posts - Popular Section
        /// </summary>
        /// <param name="categoryId">Filter by category</param>
        /// <param name="districtId">Filter by district</param>
        /// <param name="pageSize">Number of posts per page</param>
        /// <param name="pageNumber">Page number</param>
        /// <returns>Paginated list of popular posts (sorted by views)</returns>
        [HttpGet("posts/popular")]
        public async Task<IActionResult> GetPopularPosts(
            [FromQuery] int? categoryId = null,
            [FromQuery] int? districtId = null,
            [FromQuery] int pageSize = 20,
            [FromQuery] int pageNumber = 1)
        {
            var filter = new PostFilterRequest
            {
                CategoryId = categoryId,
                DistrictId = districtId,
                PageSize = Math.Min(pageSize, 100),
                PageNumber = Math.Max(pageNumber, 1)
            };

            var result = await _browseService.GetPopularPostsAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get detailed post information - Post Detail Page
        /// </summary>
        /// <param name="postId">Post ID</param>
        /// <returns>Complete post details with images and videos</returns>
        [HttpGet("posts/{postId}")]
        public async Task<IActionResult> GetPostById(int postId)
        {
            if (postId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid post ID" });
            }

            var userId = GetCurrentUserIdOrNull();
            var result = await _browseService.GetPublicPostByIdAsync(postId, userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        private int? GetCurrentUserIdOrNull()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
    }
}