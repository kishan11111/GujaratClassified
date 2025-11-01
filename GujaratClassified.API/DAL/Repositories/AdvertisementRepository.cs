using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AdvertisementRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAdvertisementAsync(Advertisement advertisement)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Title", advertisement.Title);
            parameters.Add("@Description", advertisement.Description);
            parameters.Add("@AdType", advertisement.AdType);
            parameters.Add("@Position", advertisement.Position);
            parameters.Add("@TargetUrl", advertisement.TargetUrl);
            parameters.Add("@ContactPhone", advertisement.ContactPhone);
            parameters.Add("@ContactEmail", advertisement.ContactEmail);
            parameters.Add("@CompanyName", advertisement.CompanyName);
            parameters.Add("@CompanyAddress", advertisement.CompanyAddress);
            parameters.Add("@Price", advertisement.Price);
            parameters.Add("@PriceType", advertisement.PriceType);
            parameters.Add("@StartDate", advertisement.StartDate);
            parameters.Add("@EndDate", advertisement.EndDate);
            parameters.Add("@Priority", advertisement.Priority);
            parameters.Add("@TargetDistrictId", advertisement.TargetDistrictId);
            parameters.Add("@TargetTalukaId", advertisement.TargetTalukaId);
            parameters.Add("@TargetCategoryId", advertisement.TargetCategoryId);
            parameters.Add("@TargetAgeGroup", advertisement.TargetAgeGroup);
            parameters.Add("@TargetGender", advertisement.TargetGender);

            var adId = await connection.QuerySingleAsync<int>(
                "SP_CreateAdvertisement",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return adId;
        }

        public async Task<Advertisement?> GetAdvertisementByIdAsync(int adId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);

            var advertisement = await connection.QueryFirstOrDefaultAsync<Advertisement>(
                "SP_GetAdvertisementById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return advertisement;
        }

        public async Task<bool> UpdateAdvertisementAsync(Advertisement advertisement)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", advertisement.AdId);
            parameters.Add("@Title", advertisement.Title);
            parameters.Add("@Description", advertisement.Description);
            parameters.Add("@TargetUrl", advertisement.TargetUrl);
            parameters.Add("@ContactPhone", advertisement.ContactPhone);
            parameters.Add("@ContactEmail", advertisement.ContactEmail);
            parameters.Add("@CompanyName", advertisement.CompanyName);
            parameters.Add("@CompanyAddress", advertisement.CompanyAddress);
            parameters.Add("@Price", advertisement.Price);
            parameters.Add("@StartDate", advertisement.StartDate);
            parameters.Add("@EndDate", advertisement.EndDate);
            parameters.Add("@Priority", advertisement.Priority);
            parameters.Add("@TargetDistrictId", advertisement.TargetDistrictId);
            parameters.Add("@TargetTalukaId", advertisement.TargetTalukaId);
            parameters.Add("@TargetCategoryId", advertisement.TargetCategoryId);
            parameters.Add("@TargetAgeGroup", advertisement.TargetAgeGroup);
            parameters.Add("@TargetGender", advertisement.TargetGender);

            var result = await connection.QueryFirstAsync<dynamic>(
                "SP_UpdateAdvertisement",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.RowsAffected > 0;
        }

        public async Task<bool> UpdateAdvertisementStatusAsync(int adId, string status)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);
            parameters.Add("@Status", status);

            var result = await connection.QueryFirstAsync<dynamic>(
                "SP_UpdateAdvertisementStatus",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.RowsAffected > 0;
        }

        public async Task<bool> DeleteAdvertisementAsync(int adId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);

            var result = await connection.QueryFirstAsync<dynamic>(
                "SP_DeleteAdvertisement",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.RowsAffected > 0;
        }

        public async Task<(List<Advertisement> Ads, int TotalCount)> GetAdvertisementsWithFiltersAsync(AdFilterRequest filter)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdType", filter.AdType);
            parameters.Add("@Position", filter.Position);
            parameters.Add("@Status", filter.Status);
            parameters.Add("@TargetDistrictId", filter.TargetDistrictId);
            parameters.Add("@TargetCategoryId", filter.TargetCategoryId);
            parameters.Add("@IsActive", filter.IsActive);
            parameters.Add("@IsPaid", filter.IsPaid);
            parameters.Add("@StartDate", filter.StartDate);
            parameters.Add("@EndDate", filter.EndDate);
            parameters.Add("@SearchTerm", filter.SearchTerm);
            parameters.Add("@SortBy", filter.SortBy);
            parameters.Add("@PageSize", filter.PageSize);
            parameters.Add("@PageNumber", filter.PageNumber);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetAdvertisementsWithFilters",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var ads = (await multi.ReadAsync<Advertisement>()).ToList();
            var totalCount = await multi.ReadFirstAsync<int>();

            return (ads, totalCount);
        }

        public async Task<List<Advertisement>> GetActiveAdvertisementsAsync(string position, int? userDistrictId = null,
            int? userCategoryId = null, string? userGender = null, int? userAge = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Position", position);
            parameters.Add("@UserDistrictId", userDistrictId);
            parameters.Add("@UserCategoryId", userCategoryId);
            parameters.Add("@UserGender", userGender);
            parameters.Add("@UserAge", userAge);

            var ads = await connection.QueryAsync<Advertisement>(
                "SP_GetActiveAdvertisements",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return ads.ToList();
        }

        public async Task<bool> AddAdvertisementImageAsync(int adId, string imageUrl)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);
            parameters.Add("@ImageUrl", imageUrl);

            var result = await connection.QueryFirstAsync<dynamic>(
                "SP_AddAdvertisementImage",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.RowsAffected > 0;
        }

        public async Task<bool> AddAdvertisementVideoAsync(int adId, string videoUrl)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);
            parameters.Add("@VideoUrl", videoUrl);

            var result = await connection.QueryFirstAsync<dynamic>(
                "SP_AddAdvertisementVideo",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result.RowsAffected > 0;
        }

        public async Task<bool> IncrementAdViewAsync(int adId, int? userId, string? ipAddress, string? userAgent, string position)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);
            parameters.Add("@UserId", userId);
            parameters.Add("@IpAddress", ipAddress);
            parameters.Add("@UserAgent", userAgent);
            parameters.Add("@Position", position);

            await connection.ExecuteAsync(
                "SP_IncrementAdView",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<bool> IncrementAdClickAsync(int adId, int? userId, string? ipAddress, string? userAgent, string? referrer)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);
            parameters.Add("@UserId", userId);
            parameters.Add("@IpAddress", ipAddress);
            parameters.Add("@UserAgent", userAgent);
            parameters.Add("@Referrer", referrer);

            await connection.ExecuteAsync(
                "SP_IncrementAdClick",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<AdAnalyticsResponse> GetAdvertisementAnalyticsAsync(int adId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AdId", adId);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetAdvertisementAnalytics",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var analytics = await multi.ReadFirstOrDefaultAsync<AdAnalyticsResponse>();
            if (analytics != null)
            {
                analytics.DailyStats = (await multi.ReadAsync<DailyAdStats>()).ToList();
            }

            return analytics ?? new AdAnalyticsResponse();
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

    }
}