using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Controllers.Public
{
    [Route("api/public/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        /// <summary>
        /// Get all blogs with filtering and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllBlogs([FromQuery] BlogFilterRequest filter)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }

            var result = await _blogService.GetAllBlogsAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get blog by ID with related blogs
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            var result = await _blogService.GetBlogByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Get featured blogs
        /// </summary>
        [HttpGet("featured")]
        public async Task<IActionResult> GetFeaturedBlogs([FromQuery] int count = 5)
        {
            if (count <= 0 || count > 20)
            {
                return BadRequest(new { Message = "Count must be between 1 and 20" });
            }

            var result = await _blogService.GetFeaturedBlogsAsync(count);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}