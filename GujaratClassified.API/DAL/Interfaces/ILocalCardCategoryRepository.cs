using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface ILocalCardCategoryRepository
    {
        Task<List<LocalCardCategory>> GetAllCategoriesAsync();
        Task<List<LocalCardSubCategory>> GetSubCategoriesByCategoryAsync(int categoryId);
        Task<LocalCardCategory?> GetCategoryByIdAsync(int categoryId);
        Task<LocalCardSubCategory?> GetSubCategoryByIdAsync(int subCategoryId);
        Task<int> GetCategoryCardCountAsync(int categoryId);
        Task<int> GetSubCategoryCardCountAsync(int subCategoryId);
    }
}