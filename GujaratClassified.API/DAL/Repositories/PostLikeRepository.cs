using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class PostLikeRepository : IPostLikeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PostLikeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> ToggleLikeAsync(int postId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);
            parameters.Add("@UserId", userId);
            parameters.Add("@IsLiked", dbType: DbType.Boolean, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                "SP_TogglePostLike",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return parameters.Get<bool>("@IsLiked");
        }

        //public async Task<bool> IsPostLikedByUserAsync(int postId, int userId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@PostId", postId);
        //    parameters.Add("@UserId", userId);

        //    var result = await connection.QueryFirstOrDefaultAsync<int>(
        //        "SELECT COUNT(1) FROM PostLikes WHERE PostId = @PostId AND UserId = @UserId",
        //        parameters
        //    );

        //    return result > 0;
        //}
        public async Task<bool> IsPostLikedByUserAsync(int postId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);
            parameters.Add("@UserId", userId);

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "SP_IsPostLikedByUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }


        //public async Task<int> GetPostLikeCountAsync(int postId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@PostId", postId);

        //    var count = await connection.QueryFirstOrDefaultAsync<int>(
        //        "SELECT COUNT(1) FROM PostLikes WHERE PostId = @PostId",
        //        parameters
        //    );

        //    return count;
        //}
        public async Task<int> GetPostLikeCountAsync(int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);

            var count = await connection.QueryFirstOrDefaultAsync<int>(
                "SP_GetPostLikeCount",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count;
        }

        public async Task<List<PostLike>> GetPostLikesAsync(int postId, int pageSize, int pageNumber)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);

            var likes = await connection.QueryAsync<PostLike>(
                "SP_GetPostLikes",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return likes.ToList();
        }

        public async Task<List<PostLike>> GetUserLikesAsync(int userId, int pageSize, int pageNumber)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);

            var likes = await connection.QueryAsync<PostLike>(
                "SP_GetUserLikes",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return likes.ToList();
        }
    }
}