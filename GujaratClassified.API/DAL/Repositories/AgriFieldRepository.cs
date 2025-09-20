using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using System.Data;
using System.Text.Json;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AgriFieldRepository : IAgriFieldRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AgriFieldRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateAgriFieldAsync(AgriField agriField)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", agriField.UserId);
            parameters.Add("@FarmName", agriField.FarmName);
            parameters.Add("@DistrictId", agriField.DistrictId);
            parameters.Add("@TalukaId", agriField.TalukaId);
            parameters.Add("@VillageId", agriField.VillageId);
            parameters.Add("@Address", agriField.Address);
            parameters.Add("@CropType", agriField.CropType);
            parameters.Add("@FarmingMethod", agriField.FarmingMethod);
            parameters.Add("@Season", agriField.Season);
            parameters.Add("@Title", agriField.Title);
            parameters.Add("@Description", agriField.Description);
            parameters.Add("@FarmSizeAcres", agriField.FarmSizeAcres);
            parameters.Add("@SoilType", agriField.SoilType);
            parameters.Add("@WaterSource", agriField.WaterSource);
            parameters.Add("@PlantingDate", agriField.PlantingDate);
            parameters.Add("@ExpectedHarvestDate", agriField.ExpectedHarvestDate);
            parameters.Add("@Tags", agriField.Tags);
            parameters.Add("@IsFeatured", agriField.IsFeatured);

            var agriFieldId = await connection.QuerySingleAsync<int>(
                "SP_CreateAgriField",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return agriFieldId;
        }

        public async Task<AgriField?> GetAgriFieldByIdAsync(int agriFieldId, int? currentUserId = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AgriFieldId", agriFieldId);
            parameters.Add("@CurrentUserId", currentUserId);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetAgriFieldById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var agriField = await multi.ReadFirstOrDefaultAsync<AgriField>();
            if (agriField != null)
            {
                agriField.Images = (await multi.ReadAsync<AgriFieldImage>()).ToList();
                agriField.Videos = (await multi.ReadAsync<AgriFieldVideo>()).ToList();

                // Parse JSON tags if exists
                if (!string.IsNullOrEmpty(agriField.Tags))
                {
                    try
                    {
                        // Note: You might want to add a Tags property as List<string> in response models
                    }
                    catch { /* Ignore parsing errors */ }
                }
            }

            return agriField;
        }

        public async Task<(List<AgriField> AgriFields, int TotalCount)> GetAgriFieldsAsync(AgriFieldSearchRequest request, int? currentUserId = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PageNumber", request.PageNumber);
            parameters.Add("@PageSize", request.PageSize);
            parameters.Add("@SearchKeyword", request.SearchKeyword);
            parameters.Add("@DistrictId", request.DistrictId);
            parameters.Add("@TalukaId", request.TalukaId);
            parameters.Add("@VillageId", request.VillageId);
            parameters.Add("@CropType", request.CropType);
            parameters.Add("@FarmingMethod", request.FarmingMethod);
            parameters.Add("@Season", request.Season);
            parameters.Add("@SoilType", request.SoilType);
            parameters.Add("@WaterSource", request.WaterSource);
            parameters.Add("@MinFarmSize", request.MinFarmSize);
            parameters.Add("@MaxFarmSize", request.MaxFarmSize);
            parameters.Add("@SortBy", request.SortBy);
            parameters.Add("@FeaturedOnly", request.FeaturedOnly);
            parameters.Add("@Tags", request.Tags != null ? JsonSerializer.Serialize(request.Tags) : null);
            parameters.Add("@CurrentUserId", currentUserId);

            using var multi = await connection.QueryMultipleAsync(
                "SP_SearchAgriFields",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var agriFields = (await multi.ReadAsync<AgriField>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return (agriFields, totalCount);
        }

        public async Task<(List<AgriField> AgriFields, int TotalCount)> GetUserAgriFieldsAsync(int userId, int pageNumber = 1, int pageSize = 20, string? status = null)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);
            parameters.Add("@Status", status);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetUserAgriFields",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var agriFields = (await multi.ReadAsync<AgriField>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return (agriFields, totalCount);
        }

        public async Task<bool> UpdateAgriFieldAsync(int agriFieldId, AgriField agriField)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AgriFieldId", agriFieldId);
            parameters.Add("@FarmName", agriField.FarmName);
            parameters.Add("@DistrictId", agriField.DistrictId);
            parameters.Add("@TalukaId", agriField.TalukaId);
            parameters.Add("@VillageId", agriField.VillageId);
            parameters.Add("@Address", agriField.Address);
            parameters.Add("@CropType", agriField.CropType);
            parameters.Add("@FarmingMethod", agriField.FarmingMethod);
            parameters.Add("@Season", agriField.Season);
            parameters.Add("@Title", agriField.Title);
            parameters.Add("@Description", agriField.Description);
            parameters.Add("@FarmSizeAcres", agriField.FarmSizeAcres);
            parameters.Add("@SoilType", agriField.SoilType);
            parameters.Add("@WaterSource", agriField.WaterSource);
            parameters.Add("@PlantingDate", agriField.PlantingDate);
            parameters.Add("@ExpectedHarvestDate", agriField.ExpectedHarvestDate);
            parameters.Add("@Tags", agriField.Tags);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            var result = await connection.ExecuteAsync(
                "SP_UpdateAgriField",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> DeleteAgriFieldAsync(int agriFieldId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AgriFieldId", agriFieldId);
            parameters.Add("@UserId", userId);

            var result = await connection.ExecuteAsync(
                "SP_DeleteAgriField",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> UpdateAgriFieldStatusAsync(int agriFieldId, string status)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "UPDATE AgriFields SET Status = @Status, UpdatedAt = @UpdatedAt WHERE AgriFieldId = @AgriFieldId",
                new { AgriFieldId = agriFieldId, Status = status, UpdatedAt = DateTime.UtcNow }
            );

            return result > 0;
        }

        public async Task<bool> IncrementViewCountAsync(int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "UPDATE AgriFields SET ViewCount = ViewCount + 1 WHERE AgriFieldId = @AgriFieldId",
                new { AgriFieldId = agriFieldId }
            );

            return result > 0;
        }

        public async Task<List<AgriField>> GetFeaturedAgriFieldsAsync(int limit = 10)
        {
            using var connection = _connectionFactory.CreateConnection();

            var agriFields = await connection.QueryAsync<AgriField>(
                @"SELECT TOP(@Limit) af.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as FarmerName, 
                         u.Mobile as FarmerMobile, u.ProfileImage as FarmerProfileImage, u.IsVerified as FarmerVerified,
                         d.DistrictName, t.TalukaName, v.VillageName
                  FROM AgriFields af
                  INNER JOIN Users u ON af.UserId = u.UserId
                  INNER JOIN Districts d ON af.DistrictId = d.DistrictId
                  INNER JOIN Talukas t ON af.TalukaId = t.TalukaId
                  INNER JOIN Villages v ON af.VillageId = v.VillageId
                  WHERE af.IsActive = 1 AND af.IsFeatured = 1 AND af.Status = 'ACTIVE'
                  ORDER BY af.CreatedAt DESC",
                new { Limit = limit }
            );

            return agriFields.ToList();
        }

        public async Task<List<AgriField>> GetNearbyAgriFieldsAsync(int districtId, int? talukaId = null, int limit = 20)
        {
            using var connection = _connectionFactory.CreateConnection();

            var whereClause = "af.DistrictId = @DistrictId";
            var parameters = new { DistrictId = districtId, TalukaId = talukaId, Limit = limit };

            if (talukaId.HasValue)
            {
                whereClause += " AND af.TalukaId = @TalukaId";
            }

            var agriFields = await connection.QueryAsync<AgriField>(
                $@"SELECT TOP(@Limit) af.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as FarmerName, 
                          u.Mobile as FarmerMobile, u.ProfileImage as FarmerProfileImage, u.IsVerified as FarmerVerified,
                          d.DistrictName, t.TalukaName, v.VillageName
                   FROM AgriFields af
                   INNER JOIN Users u ON af.UserId = u.UserId
                   INNER JOIN Districts d ON af.DistrictId = d.DistrictId
                   INNER JOIN Talukas t ON af.TalukaId = t.TalukaId
                   INNER JOIN Villages v ON af.VillageId = v.VillageId
                   WHERE {whereClause} AND af.IsActive = 1 AND af.Status = 'ACTIVE'
                   ORDER BY af.CreatedAt DESC",
                parameters
            );

            return agriFields.ToList();
        }

        public async Task<List<AgriField>> GetTrendingAgriFieldsAsync(int days = 7, int limit = 20)
        {
            using var connection = _connectionFactory.CreateConnection();

            var agriFields = await connection.QueryAsync<AgriField>(
                @"SELECT TOP(@Limit) af.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as FarmerName, 
                         u.Mobile as FarmerMobile, u.ProfileImage as FarmerProfileImage, u.IsVerified as FarmerVerified,
                         d.DistrictName, t.TalukaName, v.VillageName
                  FROM AgriFields af
                  INNER JOIN Users u ON af.UserId = u.UserId
                  INNER JOIN Districts d ON af.DistrictId = d.DistrictId
                  INNER JOIN Talukas t ON af.TalukaId = t.TalukaId
                  INNER JOIN Villages v ON af.VillageId = v.VillageId
                  WHERE af.IsActive = 1 AND af.Status = 'ACTIVE'
                    AND af.CreatedAt >= DATEADD(day, -@Days, GETUTCDATE())
                  ORDER BY (af.LikeCount + af.CommentCount + af.ViewCount/10.0) DESC, af.CreatedAt DESC",
                new { Days = days, Limit = limit }
            );

            return agriFields.ToList();
        }
    }
}