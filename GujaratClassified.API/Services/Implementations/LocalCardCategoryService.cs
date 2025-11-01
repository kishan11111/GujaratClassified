// Services/Implementations/LocalCardCategoryService.cs
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Implementations
{
    public class LocalCardCategoryService : ILocalCardCategoryService
    {
        private readonly ILocalCardCategoryRepository _categoryRepository;
        private readonly ILogger<LocalCardCategoryService> _logger;

        public LocalCardCategoryService(
            ILocalCardCategoryRepository categoryRepository,
            ILogger<LocalCardCategoryService> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<List<LocalCardCategoryResponse>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();

                var responses = new List<LocalCardCategoryResponse>();

                foreach (var category in categories)
                {
                    var cardCount = await _categoryRepository.GetCategoryCardCountAsync(category.CategoryId);

                    responses.Add(new LocalCardCategoryResponse
                    {
                        CategoryId = category.CategoryId,
                        CategoryNameGujarati = category.CategoryNameGujarati,
                        CategoryNameEnglish = category.CategoryNameEnglish,
                        CategoryIcon = category.CategoryIcon,
                        CategoryImage = category.CategoryImage,
                        Description = category.Description,
                        TotalCards = cardCount
                    });
                }

                return ApiResponse<List<LocalCardCategoryResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting local card categories");
                return ApiResponse<List<LocalCardCategoryResponse>>.ErrorResponse(
                    "An error occurred while fetching categories",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<LocalCardSubCategoryResponse>>> GetSubCategoriesByCategoryAsync(int categoryId)
        {
            try
            {
                // Validate category exists
                var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    return ApiResponse<List<LocalCardSubCategoryResponse>>.ErrorResponse("Category not found");
                }

                var subCategories = await _categoryRepository.GetSubCategoriesByCategoryAsync(categoryId);

                var responses = new List<LocalCardSubCategoryResponse>();

                foreach (var subCategory in subCategories)
                {
                    var cardCount = await _categoryRepository.GetSubCategoryCardCountAsync(subCategory.SubCategoryId);

                    responses.Add(new LocalCardSubCategoryResponse
                    {
                        SubCategoryId = subCategory.SubCategoryId,
                        CategoryId = subCategory.CategoryId,
                        SubCategoryNameGujarati = subCategory.SubCategoryNameGujarati,
                        SubCategoryNameEnglish = subCategory.SubCategoryNameEnglish,
                        SubCategoryIcon = subCategory.SubCategoryIcon,
                        Description = subCategory.Description,
                        TotalCards = cardCount
                    });
                }

                return ApiResponse<List<LocalCardSubCategoryResponse>>.SuccessResponse(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subcategories for category {CategoryId}", categoryId);
                return ApiResponse<List<LocalCardSubCategoryResponse>>.ErrorResponse(
                    "An error occurred while fetching subcategories",
                    new List<string> { ex.Message });
            }
        }
    }
}