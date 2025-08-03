using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public LocationRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<List<District>> GetAllDistrictsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var districts = await connection.QueryAsync<District>(
                "SP_GetAllDistricts",
                commandType: CommandType.StoredProcedure
            );

            return districts.ToList();
        }

        public async Task<List<Taluka>> GetTalukasByDistrictAsync(int districtId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@DistrictId", districtId);

            var talukas = await connection.QueryAsync<Taluka>(
                "SP_GetTalukasByDistrict",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return talukas.ToList();
        }

        public async Task<List<Village>> GetVillagesByTalukaAsync(int talukaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@TalukaId", talukaId);

            var villages = await connection.QueryAsync<Village>(
                "SP_GetVillagesByTaluka",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return villages.ToList();
        }

        public async Task<District?> GetDistrictByIdAsync(int districtId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var district = await connection.QueryFirstOrDefaultAsync<District>(
                "SELECT * FROM Districts WHERE DistrictId = @DistrictId AND IsActive = 1",
                new { DistrictId = districtId }
            );

            return district;
        }

        public async Task<Taluka?> GetTalukaByIdAsync(int talukaId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var taluka = await connection.QueryFirstOrDefaultAsync<Taluka>(
                @"SELECT t.*, d.DistrictNameEnglish AS DistrictName 
                  FROM Talukas t 
                  INNER JOIN Districts d ON t.DistrictId = d.DistrictId 
                  WHERE t.TalukaId = @TalukaId AND t.IsActive = 1",
                new { TalukaId = talukaId }
            );

            return taluka;
        }

        public async Task<Village?> GetVillageByIdAsync(int villageId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var village = await connection.QueryFirstOrDefaultAsync<Village>(
                @"SELECT v.*, t.TalukaNameEnglish AS TalukaName, d.DistrictNameEnglish AS DistrictName
                  FROM Villages v 
                  INNER JOIN Talukas t ON v.TalukaId = t.TalukaId
                  INNER JOIN Districts d ON t.DistrictId = d.DistrictId
                  WHERE v.VillageId = @VillageId AND v.IsActive = 1",
                new { VillageId = villageId }
            );

            return village;
        }
    }
}