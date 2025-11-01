// Controllers/NotificationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Request;
using System.Security.Claims;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    [Produces("application/json")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get user notifications with filtering
        /// </summary>
        /// <param name="request">Filter parameters</param>
        /// <returns>List of notifications</returns>
        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] GetNotificationsRequest request)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetUserNotificationsAsync(userId, request);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Get notification statistics
        /// </summary>
        /// <returns>Notification stats including unread count</returns>
        [HttpGet("stats")]
        public async Task<IActionResult> GetNotificationStats()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.GetNotificationStatsAsync(userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        /// <param name="notificationId">Notification ID</param>
        /// <returns>Success status</returns>
        [HttpPut("{notificationId}/read")]
        public async Task<IActionResult> MarkAsRead(int notificationId)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkAsReadAsync(userId, notificationId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        /// <returns>Success status</returns>
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.MarkAllAsReadAsync(userId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        /// <param name="notificationId">Notification ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(int notificationId)
        {
            var userId = GetCurrentUserId();
            var result = await _notificationService.DeleteNotificationAsync(userId, notificationId);

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.Parse(userIdClaim ?? "0");
        }
    }
}