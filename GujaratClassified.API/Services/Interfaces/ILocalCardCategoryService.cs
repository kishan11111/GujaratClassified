using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface ILocalCardCategoryService
    {
        Task<ApiResponse<List<LocalCardCategoryResponse>>> GetAllCategoriesAsync();
        Task<ApiResponse<List<LocalCardSubCategoryResponse>>> GetSubCategoriesByCategoryAsync(int categoryId);
    }
}