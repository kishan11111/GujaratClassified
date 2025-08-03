using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Implementations
{
    public class CategoryMasterService : ICategoryMasterService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryMasterService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<List<CategoryResponse>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllCategoriesAsync();

                var response = categories.Select(c => new CategoryResponse
                {
                    CategoryId = c.CategoryId,
                    CategoryNameGujarati = c.CategoryNameGujarati,
                    CategoryNameEnglish = c.CategoryNameEnglish,
                    CategoryIcon = c.CategoryIcon,
                    CategoryImage = c.CategoryImage,
                    Description = c.Description,
                    SortOrder = c.SortOrder,
                    IsActive = c.IsActive,
                    SubCategoryCount = c.SubCategoryCount
                }).ToList();

                return ApiResponse<List<CategoryResponse>>.SuccessResponse(response,
                    $"Retrieved {response.Count} categories successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<CategoryResponse>>.ErrorResponse(
                    "An error occurred while fetching categories",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<SubCategoryResponse>>> GetSubCategoriesByCategoryAsync(int categoryId)
        {
            try
            {
                // Validate category exists
                var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
                if (category == null)
                {
                    return ApiResponse<List<SubCategoryResponse>>.ErrorResponse("Category not found");
                }

                var subCategories = await _categoryRepository.GetSubCategoriesByCategoryAsync(categoryId);

                var response = subCategories.Select(sc => new SubCategoryResponse
                {
                    SubCategoryId = sc.SubCategoryId,
                    CategoryId = sc.CategoryId,
                    SubCategoryNameGujarati = sc.SubCategoryNameGujarati,
                    SubCategoryNameEnglish = sc.SubCategoryNameEnglish,
                    SubCategoryIcon = sc.SubCategoryIcon,
                    SubCategoryImage = sc.SubCategoryImage,
                    Description = sc.Description,
                    SortOrder = sc.SortOrder,
                    IsActive = sc.IsActive,
                    CategoryName = sc.CategoryName
                }).ToList();

                return ApiResponse<List<SubCategoryResponse>>.SuccessResponse(response,
                    $"Retrieved {response.Count} subcategories successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<SubCategoryResponse>>.ErrorResponse(
                    "An error occurred while fetching subcategories",
                    new List<string> { ex.Message });
            }
        }
    }
}