// Models/Request/NotificationRequest.cs
using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class SendNotificationToUserRequest
    {
        [Required(ErrorMessage = "User ID is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; }

        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class SendBulkNotificationRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [MaxLength(500, ErrorMessage = "Message cannot exceed 500 characters")]
        public string Message { get; set; }

        public List<int>? UserIds { get; set; } // If null, send to all users
        public string? ImageUrl { get; set; }
    }

    public class MarkNotificationAsReadRequest
    {
        [Required(ErrorMessage = "Notification ID is required")]
        public int NotificationId { get; set; }
    }

    public class GetNotificationsRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool? IsRead { get; set; } // null = all, true = read, false = unread
    }
}