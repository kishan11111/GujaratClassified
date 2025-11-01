using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CategoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var categories = await connection.QueryAsync<Category>(
                "SP_GetAllCategories",
                commandType: CommandType.StoredProcedure
            );

            return categories.ToList();
        }

        public async Task<List<SubCategory>> GetSubCategoriesByCategoryAsync(int categoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", categoryId);

            var subCategories = await connection.QueryAsync<SubCategory>(
                "SP_GetSubCategoriesByCategory",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return subCategories.ToList();
        }

        //public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var category = await connection.QueryFirstOrDefaultAsync<Category>(
        //        @"SELECT c.*, COUNT(sc.SubCategoryId) AS SubCategoryCount
        //          FROM Categories c
        //          LEFT JOIN SubCategories sc ON c.CategoryId = sc.CategoryId AND sc.IsActive = 1
        //          WHERE c.CategoryId = @CategoryId AND c.IsActive = 1
        //          GROUP BY c.CategoryId, c.CategoryNameGujarati, c.CategoryNameEnglish, 
        //                   c.CategoryIcon, c.CategoryImage, c.Description, c.SortOrder, c.IsActive, c.CreatedAt, c.UpdatedAt",
        //        new { CategoryId = categoryId }
        //    );

        //    return category;
        //}

        //public async Task<SubCategory?> GetSubCategoryByIdAsync(int subCategoryId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var subCategory = await connection.QueryFirstOrDefaultAsync<SubCategory>(
        //        @"SELECT sc.*, c.CategoryNameEnglish AS CategoryName
        //          FROM SubCategories sc
        //          INNER JOIN Categories c ON sc.CategoryId = c.CategoryId
        //          WHERE sc.SubCategoryId = @SubCategoryId AND sc.IsActive = 1",
        //        new { SubCategoryId = subCategoryId }
        //    );

        //    return subCategory;
        //}

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var category = await connection.QueryFirstOrDefaultAsync<Category>(
                "sp_Category_GetById",
                new { CategoryId = categoryId },
                commandType: CommandType.StoredProcedure
            );

            return category;
        }

        public async Task<SubCategory?> GetSubCategoryByIdAsync(int subCategoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var subCategory = await connection.QueryFirstOrDefaultAsync<SubCategory>(
                "sp_SubCategory_GetById",
                new { SubCategoryId = subCategoryId },
                commandType: CommandType.StoredProcedure
            );

            return subCategory;
        }

    }
}