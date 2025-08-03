using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public OTPRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> SendOTPAsync(string mobile, string otpCode, string purpose)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Mobile", mobile);
            parameters.Add("@OTPCode", otpCode);
            parameters.Add("@Purpose", purpose);

            var result = await connection.QueryFirstOrDefaultAsync(
                "SP_SendOTP",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result != null;
        }

        public async Task<bool> VerifyOTPAsync(string mobile, string otpCode, string purpose)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Mobile", mobile);
            parameters.Add("@OTPCode", otpCode);
            parameters.Add("@Purpose", purpose);

            var result = await connection.QueryFirstOrDefaultAsync<bool>(
                "SP_VerifyOTP",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result;
        }

        public async Task<bool> IsOTPValidAsync(string mobile, string purpose)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                @"SELECT COUNT(*) 
                  FROM UserOTPs 
                  WHERE Mobile = @Mobile 
                    AND Purpose = @Purpose 
                    AND IsUsed = 0 
                    AND ExpiryTime > GETUTCDATE()",
                new { Mobile = mobile, Purpose = purpose }
            );

            return result > 0;
        }
    }
}