using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IFarmerProfileRepository
    {
        Task<int> CreateFarmerProfileAsync(FarmerProfile profile);
        Task<FarmerProfile?> GetFarmerProfileByUserIdAsync(int userId);
        Task<bool> UpdateFarmerProfileAsync(FarmerProfile profile);
        Task<bool> UpdateFarmerStatsAsync(int userId, string statType, int increment = 1);
        Task<List<FarmerProfile>> GetTopFarmersAsync(int limit = 10, string orderBy = "TotalLikes");
        Task<bool> SetVerifiedFarmerAsync(int userId, bool isVerified);
    }
}