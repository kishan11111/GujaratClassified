using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AdInquiryRepository : IAdInquiryRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AdInquiryRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAdInquiryAsync(AdInquiry inquiry)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", inquiry.AdId);
            parameters.Add("@UserId", inquiry.UserId);
            parameters.Add("@InquirerName", inquiry.InquirerName);
            parameters.Add("@InquirerPhone", inquiry.InquirerPhone);
            parameters.Add("@InquirerEmail", inquiry.InquirerEmail);
            parameters.Add("@Message", inquiry.Message);

            var inquiryId = await connection.QuerySingleAsync<int>(
                "SP_CreateAdInquiry",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return inquiryId;
        }

        //public async Task<AdInquiry?> GetAdInquiryByIdAsync(int inquiryId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        SELECT i.*, a.Title AS AdTitle, a.CompanyName, 
        //               u.FirstName + ' ' + u.LastName AS UserName
        //        FROM AdInquiries i
        //        INNER JOIN Advertisements a ON i.AdId = a.AdId
        //        INNER JOIN Users u ON i.UserId = u.UserId
        //        WHERE i.InquiryId = @InquiryId";

        //    var inquiry = await connection.QueryFirstOrDefaultAsync<AdInquiry>(sql, new { InquiryId = inquiryId });
        //    return inquiry;
        //}
        public async Task<AdInquiry?> GetAdInquiryByIdAsync(int inquiryId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@InquiryId", inquiryId);

            var inquiry = await connection.QueryFirstOrDefaultAsync<AdInquiry>(
                "SP_GetAdInquiryById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return inquiry;
        }


        public async Task<(List<AdInquiry> Inquiries, int TotalCount)> GetAdInquiriesAsync(int? adId = null, string? status = null,
            int pageSize = 20, int pageNumber = 1)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);
            parameters.Add("@Status", status);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetAdInquiries",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var inquiries = (await multi.ReadAsync<AdInquiry>()).ToList();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (inquiries, totalCount);
        }

        //public async Task<bool> UpdateInquiryStatusAsync(int inquiryId, string status)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"
        //        UPDATE AdInquiries 
        //        SET Status = @Status, 
        //            RespondedAt = CASE WHEN @Status = 'RESPONDED' THEN GETUTCDATE() ELSE RespondedAt END
        //        WHERE InquiryId = @InquiryId";

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@InquiryId", inquiryId);
        //    parameters.Add("@Status", status);

        //    var rowsAffected = await connection.ExecuteAsync(sql, parameters);
        //    return rowsAffected > 0;
        //}

        public async Task<bool> UpdateInquiryStatusAsync(int inquiryId, string status)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@InquiryId", inquiryId);
            parameters.Add("@Status", status);

            var rowsAffected = await connection.ExecuteAsync(
                "SP_UpdateInquiryStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }


        //public async Task<List<AdInquiry>> GetUserInquiriesAsync(int userId, int pageSize = 20, int pageNumber = 1)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var offset = (pageNumber - 1) * pageSize;

        //    var sql = @"
        //        SELECT i.*, a.Title AS AdTitle, a.CompanyName
        //        FROM AdInquiries i
        //        INNER JOIN Advertisements a ON i.AdId = a.AdId
        //        WHERE i.UserId = @UserId
        //        ORDER BY i.CreatedAt DESC
        //        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

        //    var parameters = new DynamicParameters();
        //    parameters.Add("@UserId", userId);
        //    parameters.Add("@Offset", offset);
        //    parameters.Add("@PageSize", pageSize);

        //    var inquiries = await connection.QueryAsync<AdInquiry>(sql, parameters);
        //    return inquiries.ToList();
        //}

        public async Task<List<AdInquiry>> GetUserInquiriesAsync(int userId, int pageSize = 20, int pageNumber = 1)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@PageNumber", pageNumber);

            var inquiries = await connection.QueryAsync<AdInquiry>(
                "SP_GetUserInquiries",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return inquiries.ToList();
        }

    }
}