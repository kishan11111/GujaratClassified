using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByMobileAsync(string mobile);
        Task<User?> GetUserByIdAsync(int userId);
        Task<int> CreateUserAsync(User user);
        Task<bool> UpdateUserProfileAsync(User user);
        Task<bool> UpdateUserPasswordAsync(int userId, string passwordHash);
        Task<bool> UpdateRefreshTokenAsync(int userId, string refreshToken, DateTime expiry);
        Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
        Task<bool> UpdateLastLoginAsync(int userId);
        Task<bool> IsMobileExistsAsync(string mobile);
    }
}
