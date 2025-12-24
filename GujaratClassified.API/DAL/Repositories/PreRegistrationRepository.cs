using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Repositories
{
    public class PreRegistrationRepository : IPreRegistrationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PreRegistrationRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<bool> CreateAsync(PreRegistration preRegistration)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = @"
                INSERT INTO PreRegistrations (Name, Mobile, DistrictId, TalukaId, VillageId, CreatedAt, IsConverted)
                VALUES (@Name, @Mobile, @DistrictId, @TalukaId, @VillageId, @CreatedAt, @IsConverted)";

            var result = await connection.ExecuteAsync(sql, new
            {
                preRegistration.Name,
                preRegistration.Mobile,
                preRegistration.DistrictId,
                preRegistration.TalukaId,
                preRegistration.VillageId,
                preRegistration.CreatedAt,
                preRegistration.IsConverted
            });

            return result > 0;
        }

        public async Task<bool> ExistsByMobileAsync(string mobile)
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = "SELECT COUNT(1) FROM PreRegistrations WHERE Mobile = @Mobile";
            var count = await connection.ExecuteScalarAsync<int>(sql, new { Mobile = mobile });

            return count > 0;
        }

        public async Task<IEnumerable<PreRegistration>> GetAllAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var sql = "SELECT * FROM PreRegistrations ORDER BY CreatedAt DESC";
            return await connection.QueryAsync<PreRegistration>(sql);
        }
    }
}
