// Services/Interfaces/INotificationService.cs
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface INotificationService
    {
        // User notifications
        Task<ApiResponse<NotificationResponse>> CreatePostNotificationAsync(int postId, int posterId);
        Task<ApiResponse<List<NotificationResponse>>> GetUserNotificationsAsync(int userId, GetNotificationsRequest request);
        Task<ApiResponse<NotificationStatsResponse>> GetNotificationStatsAsync(int userId);
        Task<ApiResponse<bool>> MarkAsReadAsync(int userId, int notificationId);
        Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId);
        Task<ApiResponse<bool>> DeleteNotificationAsync(int userId, int notificationId);

        // Admin notifications
        Task<ApiResponse<bool>> SendNotificationToUserAsync(int adminId, SendNotificationToUserRequest request);
        Task<ApiResponse<bool>> SendBulkNotificationAsync(int adminId, SendBulkNotificationRequest request);
    }
}