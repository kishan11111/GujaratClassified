// DAL/Interfaces/INotificationRepository.cs
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.DAL.Interfaces
{
    public interface INotificationRepository
    {
        Task<int> CreateNotificationAsync(Notification notification);
        Task<bool> CreateBulkNotificationsAsync(List<Notification> notifications);
        Task<Notification?> GetNotificationByIdAsync(int notificationId);
        Task<(List<Notification> Notifications, int TotalCount)> GetUserNotificationsAsync(
            int userId, bool? isRead, int pageNumber, int pageSize);
        Task<int> GetUnreadCountAsync(int userId);
        Task<bool> MarkAsReadAsync(int notificationId, int userId);
        Task<bool> MarkAllAsReadAsync(int userId);
        Task<bool> DeleteNotificationAsync(int notificationId, int userId);
        Task<List<int>> GetAllUserIdsAsync();
    }
}