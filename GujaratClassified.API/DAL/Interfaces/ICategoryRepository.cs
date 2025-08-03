using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllCategoriesAsync();
        Task<List<SubCategory>> GetSubCategoriesByCategoryAsync(int categoryId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task<SubCategory?> GetSubCategoryByIdAsync(int subCategoryId);
    }
}
