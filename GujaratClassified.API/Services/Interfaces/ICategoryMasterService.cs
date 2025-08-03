using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface ICategoryMasterService
    {
        Task<ApiResponse<List<CategoryResponse>>> GetAllCategoriesAsync();
        Task<ApiResponse<List<SubCategoryResponse>>> GetSubCategoriesByCategoryAsync(int categoryId);
    }
}
