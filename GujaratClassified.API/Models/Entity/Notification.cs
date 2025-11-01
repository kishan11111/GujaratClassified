// Models/Entity/Notification.cs
namespace GujaratClassified.API.Models.Entity
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public string? ImageUrl { get; set; }

        // Additional properties from joins
        public string? CreatorName { get; set; }
    }
}