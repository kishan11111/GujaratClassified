// DAL/Repositories/PostRepository.cs
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PostRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreatePostAsync(Post post)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@UserId", post.UserId);
                parameters.Add("@CategoryId", post.CategoryId);
                parameters.Add("@SubCategoryId", post.SubCategoryId);
                parameters.Add("@Title", post.Title);
                parameters.Add("@Description", post.Description);
                parameters.Add("@Price", post.Price);
                parameters.Add("@PriceType", post.PriceType);
                parameters.Add("@Condition", post.Condition);
                parameters.Add("@DistrictId", post.DistrictId);
                parameters.Add("@TalukaId", post.TalukaId);
                parameters.Add("@VillageId", post.VillageId);
                parameters.Add("@Address", post.Address);
                parameters.Add("@ContactMethod", post.ContactMethod);
                parameters.Add("@ContactPhone", post.ContactPhone);
                parameters.Add("@IsFeatured", post.IsFeatured);

                var postId = await connection.QuerySingleAsync<int>(
                    "SP_CreatePost",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return postId;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public async Task<Post?> GetPostByIdAsync(int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetPostById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var post = await multi.ReadFirstOrDefaultAsync<Post>();
            if (post != null)
            {
                post.Images = (await multi.ReadAsync<PostImage>()).ToList();
                post.Videos = (await multi.ReadAsync<PostVideo>()).ToList();
            }

            return post;
        }

        public async Task<(List<Post> Posts, int TotalCount)> GetUserPostsAsync(int userId, string? status, int pageSize, int pageNumber)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@Status", status);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetUserPosts",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var posts = (await multi.ReadAsync<Post>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return (posts, totalCount);
        }

        public async Task<bool> UpdatePostAsync(Post post)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", post.PostId);
            parameters.Add("@UserId", post.UserId);
            parameters.Add("@Title", post.Title);
            parameters.Add("@Description", post.Description);
            parameters.Add("@Price", post.Price);
            parameters.Add("@PriceType", post.PriceType);
            parameters.Add("@Condition", post.Condition);
            parameters.Add("@Address", post.Address);
            parameters.Add("@ContactMethod", post.ContactMethod);
            parameters.Add("@ContactPhone", post.ContactPhone);

            var result = await connection.QuerySingleAsync<int>(
                "SP_UpdatePost",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> UpdatePostStatusAsync(int postId, int userId, string status)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);
            parameters.Add("@UserId", userId);
            parameters.Add("@Status", status);

            var result = await connection.QuerySingleAsync<int>(
                "SP_UpdatePostStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> DeletePostAsync(int postId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);
            parameters.Add("@UserId", userId);

            var result = await connection.QuerySingleAsync<int>(
                "SP_DeletePost",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<(List<Post> Posts, int TotalCount)> GetPostsWithFiltersAsync(PostFilterRequest filter)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@CategoryId", filter.CategoryId);
            parameters.Add("@SubCategoryId", filter.SubCategoryId);
            parameters.Add("@DistrictId", filter.DistrictId);
            parameters.Add("@TalukaId", filter.TalukaId);
            parameters.Add("@VillageId", filter.VillageId);
            parameters.Add("@MinPrice", filter.MinPrice);
            parameters.Add("@MaxPrice", filter.MaxPrice);
            parameters.Add("@Condition", filter.Condition);
            parameters.Add("@PriceType", filter.PriceType);
            parameters.Add("@IsFeatured", filter.IsFeatured);
            parameters.Add("@Status", filter.Status);
            parameters.Add("@SearchTerm", filter.SearchTerm);
            parameters.Add("@SortBy", filter.SortBy);
            parameters.Add("@PageSize", filter.PageSize);
            parameters.Add("@PageNumber", filter.PageNumber);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetPostsWithFilters",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var posts = (await multi.ReadAsync<Post>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return (posts, totalCount);
        }

        public async Task<bool> IncrementPostViewAsync(int postId, int? userId, string? ipAddress, string? userAgent)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);
            parameters.Add("@UserId", userId);
            parameters.Add("@IpAddress", ipAddress);
            parameters.Add("@UserAgent", userAgent);

            await connection.ExecuteAsync(
                "SP_IncrementPostView",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<(List<Post> Posts, int TotalCount)> GetUserFavoritesAsync(int userId, int pageSize, int pageNumber)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetUserFavorites",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var posts = (await multi.ReadAsync<Post>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return (posts, totalCount);
        }

        public async Task<PostStatsResponse> GetUserPostStatsAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var stats = await connection.QuerySingleAsync<PostStatsResponse>(
                "SP_GetUserPostStats",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return stats;
        }

        public async Task<bool> IncrementContactCountAsync(int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);

            await connection.ExecuteAsync(
                "SP_IncrementContactCount",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }
}




