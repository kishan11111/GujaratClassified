using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;
using System.Text.Json;

namespace GujaratClassified.API.DAL.Repositories
{
    public class FarmerProfileRepository : IFarmerProfileRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public FarmerProfileRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateFarmerProfileAsync(FarmerProfile profile)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", profile.UserId);
            parameters.Add("@FarmName", profile.FarmName);
            parameters.Add("@TotalFarmArea", profile.TotalFarmArea);
            parameters.Add("@MainCrops", profile.MainCrops);
            parameters.Add("@FarmingExperience", profile.FarmingExperience);
            parameters.Add("@SpecialtyAreas", profile.SpecialtyAreas);
            parameters.Add("@Bio", profile.Bio);
            parameters.Add("@Achievements", profile.Achievements);

            var profileId = await connection.QuerySingleAsync<int>(
                "SP_CreateFarmerProfile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return profileId;
        }

        public async Task<FarmerProfile?> GetFarmerProfileByUserIdAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var profile = await connection.QueryFirstOrDefaultAsync<FarmerProfile>(
                @"SELECT fp.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as UserName,
                         u.ProfileImage as UserProfileImage, d.DistrictName, t.TalukaName
                  FROM FarmerProfiles fp
                  INNER JOIN Users u ON fp.UserId = u.UserId
                  LEFT JOIN Districts d ON u.DistrictId = d.DistrictId
                  LEFT JOIN Talukas t ON u.TalukaId = t.TalukaId
                  WHERE fp.UserId = @UserId",
                new { UserId = userId }
            );

            return profile;
        }

        public async Task<bool> UpdateFarmerProfileAsync(FarmerProfile profile)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", profile.UserId);
            parameters.Add("@FarmName", profile.FarmName);
            parameters.Add("@TotalFarmArea", profile.TotalFarmArea);
            parameters.Add("@MainCrops", profile.MainCrops);
            parameters.Add("@FarmingExperience", profile.FarmingExperience);
            parameters.Add("@SpecialtyAreas", profile.SpecialtyAreas);
            parameters.Add("@Bio", profile.Bio);
            parameters.Add("@Achievements", profile.Achievements);
            parameters.Add("@UpdatedAt", DateTime.UtcNow);

            var result = await connection.ExecuteAsync(
                "SP_UpdateFarmerProfile",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        public async Task<bool> UpdateFarmerStatsAsync(int userId, string statType, int increment = 1)
        {
            using var connection = _connectionFactory.CreateConnection();

            var columnName = statType switch
            {
                "TotalPosts" => "TotalPosts",
                "TotalFollowers" => "TotalFollowers",
                "TotalLikes" => "TotalLikes",
                "HelpfulAnswers" => "HelpfulAnswers",
                _ => throw new ArgumentException("Invalid stat type")
            };

            var result = await connection.ExecuteAsync(
                $"UPDATE FarmerProfiles SET {columnName} = {columnName} + @Increment WHERE UserId = @UserId",
                new { UserId = userId, Increment = increment }
            );

            return result > 0;
        }

        public async Task<List<FarmerProfile>> GetTopFarmersAsync(int limit = 10, string orderBy = "TotalLikes")
        {
            using var connection = _connectionFactory.CreateConnection();

            var orderByClause = orderBy switch
            {
                "TotalLikes" => "fp.TotalLikes DESC",
                "TotalPosts" => "fp.TotalPosts DESC",
                "TotalFollowers" => "fp.TotalFollowers DESC",
                "HelpfulAnswers" => "fp.HelpfulAnswers DESC",
                _ => "fp.TotalLikes DESC"
            };

            var farmers = await connection.QueryAsync<FarmerProfile>(
                $@"SELECT TOP(@Limit) fp.*, u.FirstName + ' ' + ISNULL(u.LastName, '') as UserName,
                          u.ProfileImage as UserProfileImage, u.IsVerified, d.DistrictName, t.TalukaName
                   FROM FarmerProfiles fp
                   INNER JOIN Users u ON fp.UserId = u.UserId
                   LEFT JOIN Districts d ON u.DistrictId = d.DistrictId
                   LEFT JOIN Talukas t ON u.TalukaId = t.TalukaId
                   WHERE fp.IsVerifiedFarmer = 1
                   ORDER BY {orderByClause}",
                new { Limit = limit }
            );

            return farmers.ToList();
        }

        public async Task<bool> SetVerifiedFarmerAsync(int userId, bool isVerified)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "UPDATE FarmerProfiles SET IsVerifiedFarmer = @IsVerified, UpdatedAt = @UpdatedAt WHERE UserId = @UserId",
                new { UserId = userId, IsVerified = isVerified, UpdatedAt = DateTime.UtcNow }
            );

            return result > 0;
        }
    }
}