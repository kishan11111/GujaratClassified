using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;

namespace GujaratClassified.API.DAL.Repositories
{
    public class OTPRepository : IOTPRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILoggerFactory _loggerFactory;

        public OTPRepository(IDbConnectionFactory connectionFactory, ILoggerFactory loggerFactory)
        {
            _connectionFactory = connectionFactory;
            _loggerFactory = loggerFactory;
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

        //public async Task<bool> VerifyOTPAsync(string mobile, string otpCode, string purpose)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@Mobile", mobile);
        //    parameters.Add("@OTPCode", otpCode);
        //    parameters.Add("@Purpose", purpose);

        //    var result = await connection.QueryFirstOrDefaultAsync<bool>(
        //        "SP_VerifyOTP",
        //        parameters,
        //        commandType: CommandType.StoredProcedure
        //    );

        //    return result;
        //}


        public async Task<bool> VerifyOTPAsync(string mobile, string otpCode, string purpose)
        {
            var logger = _loggerFactory.CreateLogger<OTPRepository>();

            logger.LogInformation("OTPRepository.VerifyOTPAsync - Started for Mobile: {Mobile}, Purpose: {Purpose}",
                mobile, purpose);

            try
            {
                using var connection = _connectionFactory.CreateConnection();

                logger.LogInformation("Database connection created. Connection State: {State}",
                    connection.State);

                var parameters = new DynamicParameters();
                parameters.Add("@Mobile", mobile);
                parameters.Add("@OTPCode", otpCode);
                parameters.Add("@Purpose", purpose);

                logger.LogInformation("Executing stored procedure: SP_VerifyOTP with parameters - Mobile: {Mobile}, Purpose: {Purpose}",
                    mobile, purpose);

                var result = await connection.QueryFirstOrDefaultAsync<bool>(
                    "SP_VerifyOTP",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                logger.LogInformation("Stored procedure executed successfully. Result: {Result}", result);

                return result;
            }
            catch (SqlException sqlEx)
            {
                logger.LogError(sqlEx, "SQL ERROR in VerifyOTPAsync - Mobile: {Mobile}, SQL Error Number: {ErrorNumber}, Message: {Message}, Procedure: {Procedure}, LineNumber: {LineNumber}",
                    mobile, sqlEx.Number, sqlEx.Message, sqlEx.Procedure, sqlEx.LineNumber);
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GENERAL ERROR in VerifyOTPAsync - Mobile: {Mobile}, Exception Type: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}",
                    mobile, ex.GetType().Name, ex.Message, ex.StackTrace);
                throw;
            }
            finally
            {
                logger.LogInformation("OTPRepository.VerifyOTPAsync - Completed for Mobile: {Mobile}", mobile);
            }
        }
        //public async Task<bool> IsOTPValidAsync(string mobile, string purpose)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.QueryFirstOrDefaultAsync<int>(
        //        @"SELECT COUNT(*) 
        //          FROM UserOTPs 
        //          WHERE Mobile = @Mobile 
        //            AND Purpose = @Purpose 
        //            AND IsUsed = 0 
        //            AND ExpiryTime > GETUTCDATE()",
        //        new { Mobile = mobile, Purpose = purpose }
        //    );

        //    return result > 0;
        //}
        public async Task<bool> IsOTPValidAsync(string mobile, string purpose)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Mobile", mobile);
            parameters.Add("@Purpose", purpose);

            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "SP_IsOTPValid",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

    }
}