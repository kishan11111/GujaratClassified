using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin?> GetAdminByEmailAsync(string email);
        Task<Admin?> GetAdminByIdAsync(int adminId);
        Task<int> CreateAdminAsync(Admin admin);
        Task<bool> UpdateAdminAsync(Admin admin);
        Task<bool> UpdateRefreshTokenAsync(int adminId, string refreshToken, DateTime expiry);
        Task<Admin?> GetAdminByRefreshTokenAsync(string refreshToken);
        Task<bool> UpdateLastLoginAsync(int adminId);
        Task<bool> IsEmailExistsAsync(string email);
    }
}
