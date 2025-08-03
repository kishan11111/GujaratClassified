using System.Data;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
