using System.Data;
using Microsoft.Data.SqlClient;
using GujaratClassified.API.DAL.Interfaces;

namespace GujaratClassified.API.DAL
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly string _connectionString;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
