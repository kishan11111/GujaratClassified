using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.Helpers
{
    public interface IJwtHelper
    {
        string GenerateAccessToken(Admin admin);
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        int GetAdminIdFromToken(string token);
        int GetUserIdFromToken(string token);
        bool ValidateToken(string token);
        int GetAccessTokenExpirationMinutes();
        int GetRefreshTokenExpirationDays();
    }
}
