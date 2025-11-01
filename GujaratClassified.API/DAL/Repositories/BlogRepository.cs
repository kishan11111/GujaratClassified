using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class BlogRepository : IBlogRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public BlogRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<(List<Blog> Blogs, int TotalCount)> GetAllBlogsAsync(BlogFilterRequest filter)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@SearchKeyword", filter.SearchKeyword);
            parameters.Add("@IsActive", filter.IsActive);
            parameters.Add("@IsFeatured", filter.IsFeatured);
            parameters.Add("@PageNumber", filter.PageNumber);
            parameters.Add("@PageSize", filter.PageSize);
            parameters.Add("@SortBy", filter.SortBy);
            parameters.Add("@SortOrder", filter.SortOrder);
            parameters.Add("@TotalCount", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var blogs = await connection.QueryAsync<Blog>(
                "SP_GetAllBlogs",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var totalCount = parameters.Get<int>("@TotalCount");

            return (blogs.ToList(), totalCount);
        }

        public async Task<Blog?> GetBlogByIdAsync(int blogId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@BlogId", blogId);

            var blog = await connection.QueryFirstOrDefaultAsync<Blog>(
                "SP_GetBlogById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return blog;
        }

        public async Task<List<Blog>> GetFeaturedBlogsAsync(int count)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Count", count);

            var blogs = await connection.QueryAsync<Blog>(
                "SP_GetFeaturedBlogs",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return blogs.ToList();
        }

        public async Task<List<Blog>> GetRelatedBlogsAsync(int blogId, int count)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@BlogId", blogId);
            parameters.Add("@Count", count);

            var blogs = await connection.QueryAsync<Blog>(
                "SP_GetRelatedBlogs",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return blogs.ToList();
        }

        public async Task<bool> IncrementViewCountAsync(int blogId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@BlogId", blogId);

            var result = await connection.ExecuteAsync(
                "SP_IncrementBlogViewCount",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }
    }
}