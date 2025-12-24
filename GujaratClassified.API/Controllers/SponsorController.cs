using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/sponsor")]
    [Produces("application/json")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;

        public SponsorController(ISponsorService sponsorService)
        {
            _sponsorService = sponsorService;
        }

        /// <summary>
        /// Get app open banner for sponsor advertisement
        /// </summary>
        /// <param name="deviceId">Device identifier for tracking</param>
        /// <returns>Sponsor banner details with image path and click URL</returns>
        [HttpGet("app-open-banner")]
        public async Task<IActionResult> GetAppOpenBanner([FromQuery] string deviceId)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                return BadRequest(new { Message = "Device ID is required" });
            }

            var userId = GetCurrentUserIdOptional();
            var result = await _sponsorService.GetAppOpenBannerAsync(userId, deviceId);

            if (result.Success)
            {
                return Ok(result);
            }

            return Ok(result); // Return OK even if no sponsor, client will handle
        }

        /// <summary>
        /// Get sponsor cards for in-app display
        /// </summary>
        /// <param name="count">Number of cards to retrieve (default 5)</param>
        /// <returns>List of sponsor cards with images and links</returns>
        [HttpGet("cards")]
        public async Task<IActionResult> GetSponsorCards([FromQuery] int count = 5)
        {
            if (count < 1 || count > 20)
            {
                return BadRequest(new { Message = "Count must be between 1 and 20" });
            }

            var result = await _sponsorService.GetSponsorCardsAsync(count);
            return Ok(result);
        }

        /// <summary>
        /// Track sponsor banner/card click
        /// </summary>
        /// <param name="sponsorId">Sponsor ID</param>
        /// <param name="deviceId">Device identifier</param>
        /// <returns>Success status</returns>
        [HttpPost("track-click")]
        public async Task<IActionResult> TrackSponsorClick([FromQuery] int sponsorId, [FromQuery] string deviceId)
        {
            if (sponsorId <= 0)
            {
                return BadRequest(new { Message = "Invalid sponsor ID" });
            }

            if (string.IsNullOrEmpty(deviceId))
            {
                return BadRequest(new { Message = "Device ID is required" });
            }

            var userId = GetCurrentUserIdOptional();
            var result = await _sponsorService.TrackSponsorClickAsync(sponsorId, userId, deviceId);

            return Ok(result);
        }

        /// <summary>
        /// Get system parameter value
        /// </summary>
        /// <param name="parameterName">Parameter name</param>
        /// <returns>Parameter details</returns>
        [HttpGet("parameter/{parameterName}")]
        public async Task<IActionResult> GetSystemParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
            {
                return BadRequest(new { Message = "Parameter name is required" });
            }

            var result = await _sponsorService.GetSystemParameterAsync(parameterName);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        /// <summary>
        /// Get all active system parameters
        /// </summary>
        /// <returns>Dictionary of parameter names and values</returns>
        [HttpGet("parameters")]
        public async Task<IActionResult> GetAllSystemParameters()
        {
            var result = await _sponsorService.GetAllSystemParametersAsync();
            return Ok(result);
        }

        private int? GetCurrentUserIdOptional()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
