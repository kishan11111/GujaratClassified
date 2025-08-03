// Controllers/LocationMasterController.cs
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/master/location")]
    [Produces("application/json")]
    public class LocationMasterController : ControllerBase
    {
        private readonly ILocationMasterService _locationMasterService;

        public LocationMasterController(ILocationMasterService locationMasterService)
        {
            _locationMasterService = locationMasterService;
        }

        /// <summary>
        /// Get all Gujarat districts for dropdown
        /// </summary>
        /// <returns>List of districts</returns>
        [HttpGet("districts")]
        public async Task<IActionResult> GetAllDistricts()
        {
            var result = await _locationMasterService.GetAllDistrictsAsync();

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get talukas by district ID for dropdown
        /// </summary>
        /// <param name="districtId">District ID</param>
        /// <returns>List of talukas</returns>
        [HttpGet("talukas/{districtId}")]
        public async Task<IActionResult> GetTalukasByDistrict(int districtId)
        {
            if (districtId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid district ID" });
            }

            var result = await _locationMasterService.GetTalukasByDistrictAsync(districtId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get villages by taluka ID for dropdown
        /// </summary>
        /// <param name="talukaId">Taluka ID</param>
        /// <returns>List of villages</returns>
        [HttpGet("villages/{talukaId}")]
        public async Task<IActionResult> GetVillagesByTaluka(int talukaId)
        {
            if (talukaId <= 0)
            {
                return BadRequest(new { success = false, message = "Invalid taluka ID" });
            }

            var result = await _locationMasterService.GetVillagesByTalukaAsync(talukaId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
