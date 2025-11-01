// Controllers/Admin/AdminNotificationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    //[Authorize(Roles = "Admin")]
    [Produces("application/json")]
    public class AdminNotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public AdminNotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Send notification to a specific user
        /// </summary>
        /// <param name="request">Notification details</param>
        /// <returns>Success status</returns>
        [HttpPost("send-to-user")]
        public async Task<IActionResult> SendNotificationToUser([FromBody] SendNotificationToUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminId = GetCurrentAdminId();
            var result = await _notificationService.SendNotificationToUserAsync(adminId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Send bulk notification to multiple users or all users
        /// </summary>
        /// <param name="request">Notification details</param>
        /// <returns>Success status</returns>
        [HttpPost("send-bulk")]
        public async Task<IActionResult> SendBulkNotification([FromBody] SendBulkNotificationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var adminId = GetCurrentAdminId();
            var result = await _notificationService.SendBulkNotificationAsync(adminId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        private int GetCurrentAdminId()
        {
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(adminIdClaim ?? "0");
        }
    }
}