// DAL/Repositories/LocalCardCategoryRepository.cs
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class LocalCardCategoryRepository : ILocalCardCategoryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public LocalCardCategoryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        //public async Task<List<LocalCardCategory>> GetAllCategoriesAsync()
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        SELECT 
        //            CategoryId, 
        //            CategoryNameGujarati, 
        //            CategoryNameEnglish, 
        //            CategoryIcon, 
        //            CategoryImage, 
        //            Description, 
        //            SortOrder
        //        FROM LocalCardCategory
        //        WHERE IsActive = 1
        //        ORDER BY SortOrder, CategoryId";

        //    var categories = await connection.QueryAsync<LocalCardCategory>(sql);
        //    return categories.ToList();
        //}

        //public async Task<List<LocalCardSubCategory>> GetSubCategoriesByCategoryAsync(int categoryId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        SELECT 
        //            SubCategoryId,
        //            CategoryId,
        //            SubCategoryNameGujarati,
        //            SubCategoryNameEnglish,
        //            SubCategoryIcon,
        //            Description,
        //            SortOrder
        //        FROM LocalCardSubCategory
        //        WHERE CategoryId = @CategoryId AND IsActive = 1
        //        ORDER BY SortOrder, SubCategoryId";

        //    var subCategories = await connection.QueryAsync<LocalCardSubCategory>(sql, new { CategoryId = categoryId });
        //    return subCategories.ToList();
        //}

        //public async Task<LocalCardCategory?> GetCategoryByIdAsync(int categoryId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        SELECT 
        //            CategoryId, 
        //            CategoryNameGujarati, 
        //            CategoryNameEnglish, 
        //            CategoryIcon, 
        //            CategoryImage, 
        //            Description
        //        FROM LocalCardCategory
        //        WHERE CategoryId = @CategoryId AND IsActive = 1";

        //    return await connection.QueryFirstOrDefaultAsync<LocalCardCategory>(sql, new { CategoryId = categoryId });
        //}

        //public async Task<LocalCardSubCategory?> GetSubCategoryByIdAsync(int subCategoryId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        SELECT 
        //            SubCategoryId,
        //            CategoryId,
        //            SubCategoryNameGujarati,
        //            SubCategoryNameEnglish,
        //            SubCategoryIcon,
        //            Description
        //        FROM LocalCardSubCategory
        //        WHERE SubCategoryId = @SubCategoryId AND IsActive = 1";

        //    return await connection.QueryFirstOrDefaultAsync<LocalCardSubCategory>(sql, new { SubCategoryId = subCategoryId });
        //}

        //public async Task<int> GetCategoryCardCountAsync(int categoryId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = "SELECT COUNT(*) FROM LocalCard WHERE CategoryId = @CategoryId AND IsActive = 1";

        //    return await connection.ExecuteScalarAsync<int>(sql, new { CategoryId = categoryId });
        //}

        //public async Task<int> GetSubCategoryCardCountAsync(int subCategoryId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = "SELECT COUNT(*) FROM LocalCard WHERE SubCategoryId = @SubCategoryId AND IsActive = 1";

        //    return await connection.ExecuteScalarAsync<int>(sql, new { SubCategoryId = subCategoryId });
        //}
        public async Task<List<LocalCardCategory>> GetAllCategoriesAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var categories = await connection.QueryAsync<LocalCardCategory>(
                "SP_LocalCardCategory_GetAll",
                commandType: CommandType.StoredProcedure
            );

            return categories.ToList();
        }

        public async Task<List<LocalCardSubCategory>> GetSubCategoriesByCategoryAsync(int categoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", categoryId);

            var subCategories = await connection.QueryAsync<LocalCardSubCategory>(
                "SP_LocalCardSubCategory_GetByCategory",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return subCategories.ToList();
        }

        public async Task<LocalCardCategory?> GetCategoryByIdAsync(int categoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", categoryId);

            return await connection.QueryFirstOrDefaultAsync<LocalCardCategory>(
                "SP_LocalCardCategory_GetById",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<LocalCardSubCategory?> GetSubCategoryByIdAsync(int subCategoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@SubCategoryId", subCategoryId);

            return await connection.QueryFirstOrDefaultAsync<LocalCardSubCategory>(
                "SP_LocalCardSubCategory_GetById",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> GetCategoryCardCountAsync(int categoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", categoryId);

            return await connection.ExecuteScalarAsync<int>(
                "SP_LocalCardCategory_GetCardCount",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<int> GetSubCategoryCardCountAsync(int subCategoryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@SubCategoryId", subCategoryId);

            return await connection.ExecuteScalarAsync<int>(
                "SP_LocalCardSubCategory_GetCardCount",
                parameters,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}