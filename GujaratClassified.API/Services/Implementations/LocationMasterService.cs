using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Implementations
{
    public class LocationMasterService : ILocationMasterService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationMasterService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<ApiResponse<List<DistrictResponse>>> GetAllDistrictsAsync()
        {
            try
            {
                var districts = await _locationRepository.GetAllDistrictsAsync();

                var response = districts.Select(d => new DistrictResponse
                {
                    DistrictId = d.DistrictId,
                    DistrictNameGujarati = d.DistrictNameGujarati,
                    DistrictNameEnglish = d.DistrictNameEnglish,
                    IsActive = d.IsActive
                }).ToList();

                return ApiResponse<List<DistrictResponse>>.SuccessResponse(response,
                    $"Retrieved {response.Count} districts successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<DistrictResponse>>.ErrorResponse(
                    "An error occurred while fetching districts",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<TalukaResponse>>> GetTalukasByDistrictAsync(int districtId)
        {
            try
            {
                // Validate district exists
                var district = await _locationRepository.GetDistrictByIdAsync(districtId);
                if (district == null)
                {
                    return ApiResponse<List<TalukaResponse>>.ErrorResponse("District not found");
                }

                var talukas = await _locationRepository.GetTalukasByDistrictAsync(districtId);

                var response = talukas.Select(t => new TalukaResponse
                {
                    TalukaId = t.TalukaId,
                    DistrictId = t.DistrictId,
                    TalukaNameGujarati = t.TalukaNameGujarati,
                    TalukaNameEnglish = t.TalukaNameEnglish,
                    DistrictName = t.DistrictName,
                    IsActive = t.IsActive
                }).ToList();

                return ApiResponse<List<TalukaResponse>>.SuccessResponse(response,
                    $"Retrieved {response.Count} talukas successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<TalukaResponse>>.ErrorResponse(
                    "An error occurred while fetching talukas",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<VillageResponse>>> GetVillagesByTalukaAsync(int talukaId)
        {
            try
            {
                // Validate taluka exists
                var taluka = await _locationRepository.GetTalukaByIdAsync(talukaId);
                if (taluka == null)
                {
                    return ApiResponse<List<VillageResponse>>.ErrorResponse("Taluka not found");
                }

                var villages = await _locationRepository.GetVillagesByTalukaAsync(talukaId);

                var response = villages.Select(v => new VillageResponse
                {
                    VillageId = v.VillageId,
                    TalukaId = v.TalukaId,
                    VillageNameGujarati = v.VillageNameGujarati,
                    VillageNameEnglish = v.VillageNameEnglish,
                    TalukaName = v.TalukaName,
                    DistrictName = v.DistrictName,
                    IsActive = v.IsActive
                }).ToList();

                return ApiResponse<List<VillageResponse>>.SuccessResponse(response,
                    $"Retrieved {response.Count} villages successfully");
            }
            catch (Exception ex)
            {
                return ApiResponse<List<VillageResponse>>.ErrorResponse(
                    "An error occurred while fetching villages",
                    new List<string> { ex.Message });
            }
        }
    }
}