// DAL/Repositories/AdminRepository.cs
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AdminRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Admin?> GetAdminByEmailAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);

            var admin = await connection.QueryFirstOrDefaultAsync<Admin>(
                "SP_GetAdminByEmail",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return admin;
        }

        public async Task<Admin?> GetAdminByIdAsync(int adminId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdminId", adminId);

            var admin = await connection.QueryFirstOrDefaultAsync<Admin>(
                "SP_GetAdminById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return admin;
        }

        public async Task<int> CreateAdminAsync(Admin admin)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@FirstName", admin.FirstName);
            parameters.Add("@LastName", admin.LastName);
            parameters.Add("@Email", admin.Email);
            parameters.Add("@Mobile", admin.Mobile);
            parameters.Add("@PasswordHash", admin.PasswordHash);
            parameters.Add("@IsSuperAdmin", admin.IsSuperAdmin);

            var adminId = await connection.QuerySingleAsync<int>(
                "SP_CreateAdmin",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return adminId;
        }

        public async Task<bool> UpdateAdminAsync(Admin admin)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdminId", admin.AdminId);
            parameters.Add("@FirstName", admin.FirstName);
            parameters.Add("@LastName", admin.LastName);
            parameters.Add("@Mobile", admin.Mobile);
            parameters.Add("@ProfileImage", admin.ProfileImage);
            parameters.Add("@PasswordHash", admin.PasswordHash);

            var result = await connection.ExecuteAsync(
                "SP_UpdateAdmin",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> UpdateRefreshTokenAsync(int adminId, string refreshToken, DateTime expiry)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdminId", adminId);
            parameters.Add("@RefreshToken", refreshToken);
            parameters.Add("@RefreshTokenExpiry", expiry);

            var result = await connection.ExecuteAsync(
                "SP_UpdateAdminRefreshToken",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<Admin?> GetAdminByRefreshTokenAsync(string refreshToken)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@RefreshToken", refreshToken);

            var admin = await connection.QueryFirstOrDefaultAsync<Admin>(
                "SP_GetAdminByRefreshToken",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return admin;
        }

        public async Task<bool> UpdateLastLoginAsync(int adminId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdminId", adminId);

            var result = await connection.ExecuteAsync(
                "SP_UpdateAdminLastLogin",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Email", email);

            var count = await connection.QuerySingleAsync<int>(
                "SP_IsEmailExists",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count > 0;
        }
    }
}