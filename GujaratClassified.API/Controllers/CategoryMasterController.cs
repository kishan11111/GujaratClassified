using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/master/category")]
    [Produces("application/json")]
    public class CategoryMasterController : ControllerBase
    {
        private readonly ICategoryMasterService _categoryMasterService;

        public CategoryMasterController(ICategoryMasterService categoryMasterService)
        {
            _categoryMasterService = categoryMasterService;
        }

        /// <summary>
        /// Get all active categories for mobile app grid/list
        /// </summary>
        /// <returns>List of categories with subcategory count</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryMasterService.GetAllCategoriesAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get subcategories by category ID for dropdown
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of subcategories</returns>
        [HttpGet("{categoryId}/subcategories")]
        public async Task<IActionResult> GetSubCategoriesByCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid category ID" });
            }

            var result = await _categoryMasterService.GetSubCategoriesByCategoryAsync(categoryId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}