// DAL/Repositories/FavoriteRepository.cs
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class FavoriteRepository : IFavoriteRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FavoriteRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<string> ToggleFavoriteAsync(int userId, int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PostId", postId);

            var action = await connection.QuerySingleAsync<string>(
                "SP_ToggleFavorite",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return action;
        }

        //public async Task<bool> IsFavoriteAsync(int userId, int postId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var count = await connection.QuerySingleAsync<int>(
        //        "SELECT COUNT(*) FROM UserFavorites WHERE UserId = @UserId AND PostId = @PostId",
        //        new { UserId = userId, PostId = postId }
        //    );

        //    return count > 0;
        //}
        public async Task<bool> IsFavoriteAsync(int userId, int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PostId", postId);

            var count = await connection.QuerySingleAsync<int>(
                "SP_IsFavorite",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }

    }
}