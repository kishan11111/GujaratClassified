using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AgriFieldLikeRepository : IAgriFieldLikeRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AgriFieldLikeRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> LikeAgriFieldAsync(int agriFieldId, int userId, string? reactionType = "LIKE")
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AgriFieldId", agriFieldId);
                parameters.Add("@UserId", userId);
                parameters.Add("@ReactionType", reactionType);

                var result = await connection.ExecuteAsync(
                    "SP_LikeAgriField",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                if (result > 0)
                {
                    await UpdateLikeCountAsync(agriFieldId, 1);
                }

                return result > 0;
            }
            catch (Exception)
            {
                // Handle duplicate key constraint (user already liked)
                return false;
            }
        }

        //public async Task<bool> UnlikeAgriFieldAsync(int agriFieldId, int userId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "DELETE FROM AgriFieldLikes WHERE AgriFieldId = @AgriFieldId AND UserId = @UserId",
        //        new { AgriFieldId = agriFieldId, UserId = userId }
        //    );

        //    if (result > 0)
        //    {
        //        await UpdateLikeCountAsync(agriFieldId, -1);
        //    }

        //    return result > 0;
        //}
        public async Task<bool> UnlikeAgriFieldAsync(int agriFieldId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<int>(
                "SP_UnlikeAgriField",
                new { AgriFieldId = agriFieldId, UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            if (result > 0)
                await UpdateLikeCountAsync(agriFieldId, -1);

            return result > 0;
        }


        //public async Task<bool> IsLikedByUserAsync(int agriFieldId, int userId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var count = await connection.QuerySingleAsync<int>(
        //        "SELECT COUNT(1) FROM AgriFieldLikes WHERE AgriFieldId = @AgriFieldId AND UserId = @UserId",
        //        new { AgriFieldId = agriFieldId, UserId = userId }
        //    );

        //    return count > 0;
        //}
        public async Task<bool> IsLikedByUserAsync(int agriFieldId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var count = await connection.QuerySingleAsync<int>(
                "SP_IsAgriFieldLikedByUser",
                new { AgriFieldId = agriFieldId, UserId = userId },
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }

        //public async Task<List<AgriFieldLike>> GetAgriFieldLikesAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var offset = (pageNumber - 1) * pageSize;

        //    var likes = await connection.QueryAsync<AgriFieldLike>(
        //        @"SELECT l.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as UserName, 
        //                 u.ProfileImage as UserProfileImage
        //          FROM AgriFieldLikes l
        //          INNER JOIN Users u ON l.UserId = u.UserId
        //          WHERE l.AgriFieldId = @AgriFieldId
        //          ORDER BY l.CreatedAt DESC
        //          OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
        //        new { AgriFieldId = agriFieldId, Offset = offset, PageSize = pageSize }
        //    );

        //    return likes.ToList();
        //}

        public async Task<List<AgriFieldLike>> GetAgriFieldLikesAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50)
        {
            using var connection = _connectionFactory.CreateConnection();
            var offset = (pageNumber - 1) * pageSize;

            var likes = await connection.QueryAsync<AgriFieldLike>(
                "SP_GetAgriFieldLikes",
                new { AgriFieldId = agriFieldId, Offset = offset, PageSize = pageSize },
                commandType: CommandType.StoredProcedure
            );

            return likes.ToList();
        }


        //public async Task<int> GetLikeCountAsync(int agriFieldId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var count = await connection.QuerySingleAsync<int>(
        //        "SELECT COUNT(1) FROM AgriFieldLikes WHERE AgriFieldId = @AgriFieldId",
        //        new { AgriFieldId = agriFieldId }
        //    );

        //    return count;
        //}

        public async Task<int> GetLikeCountAsync(int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            return await connection.QuerySingleAsync<int>(
                "SP_GetAgriFieldLikeCount",
                new { AgriFieldId = agriFieldId },
                commandType: CommandType.StoredProcedure
            );
        }

        //public async Task<bool> UpdateLikeCountAsync(int agriFieldId, int increment = 1)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "UPDATE AgriFields SET LikeCount = LikeCount + @Increment WHERE AgriFieldId = @AgriFieldId",
        //        new { AgriFieldId = agriFieldId, Increment = increment }
        //    );

        //    return result > 0;
        //}

        public async Task<bool> UpdateLikeCountAsync(int agriFieldId, int increment = 1)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QuerySingleAsync<int>(
                "SP_UpdateAgriFieldLikeCount",
                new { AgriFieldId = agriFieldId, Increment = increment },
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

    }
}