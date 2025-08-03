using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface ILocationMasterService
    {
        Task<ApiResponse<List<DistrictResponse>>> GetAllDistrictsAsync();
        Task<ApiResponse<List<TalukaResponse>>> GetTalukasByDistrictAsync(int districtId);
        Task<ApiResponse<List<VillageResponse>>> GetVillagesByTalukaAsync(int talukaId);
    }
}