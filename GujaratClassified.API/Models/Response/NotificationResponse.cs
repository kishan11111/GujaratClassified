// Models/Response/NotificationResponse.cs
namespace GujaratClassified.API.Models.Response
{
    public class NotificationResponse
    {
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string NotificationType { get; set; }
        public int? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageUrl { get; set; }
        public string? CreatorName { get; set; }
    }

    public class NotificationStatsResponse
    {
        public int TotalNotifications { get; set; }
        public int UnreadCount { get; set; }
        public int ReadCount { get; set; }
    }
}