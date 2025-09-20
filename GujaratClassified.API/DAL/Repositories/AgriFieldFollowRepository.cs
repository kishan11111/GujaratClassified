using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AgriFieldFollowRepository : IAgriFieldFollowRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AgriFieldFollowRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> FollowAgriFieldAsync(int agriFieldId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            try
            {
                var result = await connection.ExecuteAsync(
                    @"INSERT INTO AgriFieldFollows (AgriFieldId, FollowerUserId, IsActive, CreatedAt) 
                      VALUES (@AgriFieldId, @UserId, 1, @CreatedAt)",
                    new { AgriFieldId = agriFieldId, UserId = userId, CreatedAt = DateTime.UtcNow }
                );

                if (result > 0)
                {
                    await UpdateFollowerCountAsync(agriFieldId, 1);
                }

                return result > 0;
            }
            catch (Exception)
            {
                // Handle duplicate key constraint (user already following)
                return false;
            }
        }

        public async Task<bool> UnfollowAgriFieldAsync(int agriFieldId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "UPDATE AgriFieldFollows SET IsActive = 0 WHERE AgriFieldId = @AgriFieldId AND FollowerUserId = @UserId",
                new { AgriFieldId = agriFieldId, UserId = userId }
            );

            if (result > 0)
            {
                await UpdateFollowerCountAsync(agriFieldId, -1);
            }

            return result > 0;
        }

        public async Task<bool> IsFollowedByUserAsync(int agriFieldId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var count = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(1) FROM AgriFieldFollows WHERE AgriFieldId = @AgriFieldId AND FollowerUserId = @UserId AND IsActive = 1",
                new { AgriFieldId = agriFieldId, UserId = userId }
            );

            return count > 0;
        }

        public async Task<List<AgriFieldFollow>> GetFollowersAsync(int agriFieldId, int pageNumber = 1, int pageSize = 50)
        {
            using var connection = _connectionFactory.CreateConnection();

            var offset = (pageNumber - 1) * pageSize;

            var followers = await connection.QueryAsync<AgriFieldFollow>(
                @"SELECT f.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as FollowerName, 
                         u.ProfileImage as FollowerProfileImage, u.IsVerified as FollowerVerified
                  FROM AgriFieldFollows f
                  INNER JOIN Users u ON f.FollowerUserId = u.UserId
                  WHERE f.AgriFieldId = @AgriFieldId AND f.IsActive = 1
                  ORDER BY f.CreatedAt DESC
                  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
                new { AgriFieldId = agriFieldId, Offset = offset, PageSize = pageSize }
            );

            return followers.ToList();
        }

        public async Task<int> GetFollowerCountAsync(int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var count = await connection.QuerySingleAsync<int>(
                "SELECT COUNT(1) FROM AgriFieldFollows WHERE AgriFieldId = @AgriFieldId AND IsActive = 1",
                new { AgriFieldId = agriFieldId }
            );

            return count;
        }

        public async Task<bool> UpdateFollowerCountAsync(int agriFieldId, int increment = 1)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "UPDATE AgriFields SET FollowerCount = FollowerCount + @Increment WHERE AgriFieldId = @AgriFieldId",
                new { AgriFieldId = agriFieldId, Increment = increment }
            );

            return result > 0;
        }

        public async Task<List<AgriField>> GetFollowedAgriFieldsAsync(int userId, int pageNumber = 1, int pageSize = 20)
        {
            using var connection = _connectionFactory.CreateConnection();

            var offset = (pageNumber - 1) * pageSize;

            var agriFields = await connection.QueryAsync<AgriField>(
                @"SELECT af.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as FarmerName, 
                         u.Mobile as FarmerMobile, u.ProfileImage as FarmerProfileImage, u.IsVerified as FarmerVerified,
                         d.DistrictName, t.TalukaName, v.VillageName
                  FROM AgriFieldFollows f
                  INNER JOIN AgriFields af ON f.AgriFieldId = af.AgriFieldId
                  INNER JOIN Users u ON af.UserId = u.UserId
                  INNER JOIN Districts d ON af.DistrictId = d.DistrictId
                  INNER JOIN Talukas t ON af.TalukaId = t.TalukaId
                  INNER JOIN Villages v ON af.VillageId = v.VillageId
                  WHERE f.FollowerUserId = @UserId AND f.IsActive = 1 
                    AND af.IsActive = 1 AND af.Status = 'ACTIVE'
                  ORDER BY af.CreatedAt DESC
                  OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY",
                new { UserId = userId, Offset = offset, PageSize = pageSize }
            );

            return agriFields.ToList();
        }
    }
}
