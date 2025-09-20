using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;

namespace GujaratClassified.API.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAdvertisementController : ControllerBase
    {
        private readonly IAdvertisementService _advertisementService;

        public AdminAdvertisementController(IAdvertisementService advertisementService)
        {
            _advertisementService = advertisementService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdvertisement([FromBody] CreateAdvertisementRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }

            var result = await _advertisementService.CreateAdvertisementAsync(request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdvertisement(int id)
        {
            var result = await _advertisementService.GetAdvertisementByIdAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return NotFound(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAdvertisements([FromQuery] AdFilterRequest filter)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }

            var result = await _advertisementService.GetAdvertisementsWithFiltersAsync(filter);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvertisement(int id, [FromBody] UpdateAdvertisementRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }

            var result = await _advertisementService.UpdateAdvertisementAsync(id, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateAdvertisementStatus(int id, [FromBody] AdStatusRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(new { Message = "Validation failed", Errors = errors });
            }

            var result = await _advertisementService.UpdateAdvertisementStatusAsync(id, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvertisement(int id)
        {
            var result = await _advertisementService.DeleteAdvertisementAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("{id}/image")]
        public async Task<IActionResult> UploadAdvertisementImage(int id, IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return BadRequest(new { Message = "Image file is required", Errors = new[] { "No file uploaded" } });
            }

            var result = await _advertisementService.UploadAdvertisementImageAsync(id, imageFile);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("{id}/video")]
        public async Task<IActionResult> UploadAdvertisementVideo(int id, IFormFile videoFile)
        {
            if (videoFile == null || videoFile.Length == 0)
            {
                return BadRequest(new { Message = "Video file is required", Errors = new[] { "No file uploaded" } });
            }

            var result = await _advertisementService.UploadAdvertisementVideoAsync(id, videoFile);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("{id}/analytics")]
        public async Task<IActionResult> GetAdvertisementAnalytics(int id)
        {
            var result = await _advertisementService.GetAdvertisementAnalyticsAsync(id);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpGet("inquiries")]
        public async Task<IActionResult> GetAdInquiries([FromQuery] int? adId, [FromQuery] string? status,
            [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1)
        {
            if (pageSize <= 0 || pageSize > 100)
            {
                return BadRequest(new { Message = "Page size must be between 1 and 100", Errors = new[] { "Invalid page size" } });
            }

            if (pageNumber <= 0)
            {
                return BadRequest(new { Message = "Page number must be greater than 0", Errors = new[] { "Invalid page number" } });
            }

            var result = await _advertisementService.GetAdInquiriesAsync(adId, status, pageSize, pageNumber);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPatch("inquiries/{inquiryId}/status")]
        public async Task<IActionResult> UpdateInquiryStatus(int inquiryId, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest(new { Message = "Status is required", Errors = new[] { "Invalid status" } });
            }

            var validStatuses = new[] { "NEW", "READ", "RESPONDED", "CLOSED" };
            if (!validStatuses.Contains(status.ToUpper()))
            {
                return BadRequest(new { Message = "Invalid status", Errors = new[] { "Status must be one of: NEW, READ, RESPONDED, CLOSED" } });
            }

            var result = await _advertisementService.UpdateInquiryStatusAsync(inquiryId, status.ToUpper());

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("{id}/track/view")]
        public async Task<IActionResult> TrackAdView(int id, [FromBody] string position)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();

            var result = await _advertisementService.TrackAdViewAsync(id, null, ipAddress, userAgent, position);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPost("{id}/track/click")]
        public async Task<IActionResult> TrackAdClick(int id)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = HttpContext.Request.Headers["User-Agent"].FirstOrDefault();
            var referrer = HttpContext.Request.Headers["Referer"].FirstOrDefault();

            var result = await _advertisementService.TrackAdClickAsync(id, null, ipAddress, userAgent, referrer);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}