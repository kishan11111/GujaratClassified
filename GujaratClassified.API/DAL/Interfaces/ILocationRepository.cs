using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface ILocationRepository
    {
        Task<List<District>> GetAllDistrictsAsync();
        Task<List<Taluka>> GetTalukasByDistrictAsync(int districtId);
        Task<List<Village>> GetVillagesByTalukaAsync(int talukaId);
        Task<District?> GetDistrictByIdAsync(int districtId);
        Task<Taluka?> GetTalukaByIdAsync(int talukaId);
        Task<Village?> GetVillageByIdAsync(int villageId);
    }
}
