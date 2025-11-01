// Controllers/LocalCardCategoryController.cs
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/localcard/category")]
    [Produces("application/json")]
    public class LocalCardCategoryController : ControllerBase
    {
        private readonly ILocalCardCategoryService _categoryService;

        public LocalCardCategoryController(ILocalCardCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// બધી કેટેગરી જુઓ (Get all active categories)
        /// </summary>
        /// <returns>List of categories with card count</returns>
        [HttpGet("list")]
        public async Task<IActionResult> GetAllCategories()
        {
            var result = await _categoryService.GetAllCategoriesAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// કેટેગરી મુજબ સબકેટેગરી જુઓ (Get subcategories by category)
        /// </summary>
        /// <param name="categoryId">Category ID</param>
        /// <returns>List of subcategories with card count</returns>
        [HttpGet("{categoryId}/subcategories")]
        public async Task<IActionResult> GetSubCategoriesByCategory(int categoryId)
        {
            if (categoryId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid category ID" });
            }

            var result = await _categoryService.GetSubCategoriesByCategoryAsync(categoryId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}