using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User?> GetUserByMobileAsync(string mobile)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Mobile", mobile);

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SP_GetUserByMobile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                "SP_GetUserById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return user;
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", user.FirstName);
            parameters.Add("@LastName", user.LastName);
            parameters.Add("@Mobile", user.Mobile);
            parameters.Add("@Email", user.Email);
            parameters.Add("@PasswordHash", user.PasswordHash);
            parameters.Add("@DistrictId", user.DistrictId);
            parameters.Add("@TalukaId", user.TalukaId);
            parameters.Add("@VillageId", user.VillageId);
            parameters.Add("@DateOfBirth", user.DateOfBirth);
            parameters.Add("@Gender", user.Gender);

            var userId = await connection.QuerySingleAsync<int>(
                "SP_CreateUser",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return userId;
        }

        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", user.UserId);
            parameters.Add("@FirstName", user.FirstName);
            parameters.Add("@LastName", user.LastName);
            parameters.Add("@Email", user.Email);
            parameters.Add("@DistrictId", user.DistrictId);
            parameters.Add("@TalukaId", user.TalukaId);
            parameters.Add("@VillageId", user.VillageId);
            parameters.Add("@DateOfBirth", user.DateOfBirth);
            parameters.Add("@Gender", user.Gender);
            parameters.Add("@ProfileImage", user.ProfileImage);

            var result = await connection.ExecuteAsync(
                "SP_UpdateUserProfile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> UpdateUserPasswordAsync(int userId, string passwordHash)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PasswordHash", passwordHash);

            var result = await connection.ExecuteAsync(
                "SP_UpdateUserPassword",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@RefreshToken", refreshToken);
            parameters.Add("@RefreshTokenExpiry", expiry);

            var result = await connection.ExecuteAsync(
                "SP_UpdateUserRefreshToken",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var user = await connection.QueryFirstOrDefaultAsync<User>(
                @"SELECT u.UserId, u.FirstName, u.LastName, u.Mobile, u.Email, u.PasswordHash,
                         u.DistrictId, u.TalukaId, u.VillageId, u.ProfileImage, u.DateOfBirth, u.Gender,
                         u.IsActive, u.IsVerified, u.IsPremium, u.CreatedAt, u.UpdatedAt, u.LastLoginAt,
                         u.RefreshToken, u.RefreshTokenExpiry,
                         d.DistrictNameEnglish AS DistrictName,
                         t.TalukaNameEnglish AS TalukaName,
                         v.VillageNameEnglish AS VillageName
                  FROM Users u
                  INNER JOIN Districts d ON u.DistrictId = d.DistrictId
                  INNER JOIN Talukas t ON u.TalukaId = t.TalukaId
                  INNER JOIN Villages v ON u.VillageId = v.VillageId
                  WHERE u.RefreshToken = @RefreshToken 
                    AND u.RefreshTokenExpiry > GETUTCDATE() 
                    AND u.IsActive = 1",
                new { RefreshToken = refreshToken }
            );

            return user;
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var result = await connection.ExecuteAsync(
                "SP_UpdateUserLastLogin",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Mobile", mobile);

            var count = await connection.QuerySingleAsync<int>(
                "SP_IsMobileExists",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }
    }
}