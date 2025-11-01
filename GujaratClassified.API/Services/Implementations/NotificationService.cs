// Services/Implementations/NotificationService.cs
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            IPostRepository postRepository,
            IUserRepository userRepository,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<NotificationResponse>> CreatePostNotificationAsync(int postId, int posterId)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null)
                {
                    return ApiResponse<NotificationResponse>.ErrorResponse("Post not found");
                }

                var poster = await _userRepository.GetUserByIdAsync(posterId);
                if (poster == null)
                {
                    return ApiResponse<NotificationResponse>.ErrorResponse("User not found");
                }

                // Get all active user IDs except the poster
                var allUserIds = await _notificationRepository.GetAllUserIdsAsync();
                var targetUserIds = allUserIds.Where(id => id != posterId).ToList();

                // Create notifications for all users
                var notifications = targetUserIds.Select(userId => new Notification
                {
                    UserId = userId,
                    Title = "New Post Added",
                    Message = $"{poster.FirstName} {poster.LastName} added a new post: {post.Title}",
                    NotificationType = "POST_ADDED",
                    ReferenceId = postId,
                    ReferenceType = "Post",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    ImageUrl = post.Images?.FirstOrDefault()?.ImageUrl
                }).ToList();

                await _notificationRepository.CreateBulkNotificationsAsync(notifications);

                return ApiResponse<NotificationResponse>.SuccessResponse(
                    null,
                    $"Notification sent to {targetUserIds.Count} users"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post notification for post {PostId}", postId);
                return ApiResponse<NotificationResponse>.ErrorResponse(
                    "An error occurred while sending notifications",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<NotificationResponse>>> GetUserNotificationsAsync(
            int userId, GetNotificationsRequest request)
        {
            try
            {
                var (notifications, totalCount) = await _notificationRepository.GetUserNotificationsAsync(
                    userId,
                    request.IsRead,
                    request.PageNumber,
                    request.PageSize
                );

                var response = notifications.Select(n => new NotificationResponse
                {
                    NotificationId = n.NotificationId,
                    Title = n.Title,
                    Message = n.Message,
                    NotificationType = n.NotificationType,
                    ReferenceId = n.ReferenceId,
                    ReferenceType = n.ReferenceType,
                    IsRead = n.IsRead,
                    CreatedAt = n.CreatedAt,
                    ImageUrl = n.ImageUrl,
                    CreatorName = n.CreatorName
                }).ToList();

                var paginationInfo = new
                {
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
                };

                var result = ApiResponse<List<NotificationResponse>>.SuccessResponse(response);
                result.Pagination = paginationInfo;

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
                return ApiResponse<List<NotificationResponse>>.ErrorResponse(
                    "An error occurred while retrieving notifications",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<NotificationStatsResponse>> GetNotificationStatsAsync(int userId)
        {
            try
            {
                var unreadCount = await _notificationRepository.GetUnreadCountAsync(userId);

                var stats = new NotificationStatsResponse
                {
                    UnreadCount = unreadCount,
                    TotalNotifications = 0, // Can be fetched if needed
                    ReadCount = 0 // Can be calculated if needed
                };

                return ApiResponse<NotificationStatsResponse>.SuccessResponse(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting notification stats for user {UserId}", userId);
                return ApiResponse<NotificationStatsResponse>.ErrorResponse(
                    "An error occurred while retrieving notification stats",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> MarkAsReadAsync(int userId, int notificationId)
        {
            try
            {
                var success = await _notificationRepository.MarkAsReadAsync(notificationId, userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Notification not found or already read");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Notification marked as read");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read", notificationId);
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while marking notification as read",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> MarkAllAsReadAsync(int userId)
        {
            try
            {
                var success = await _notificationRepository.MarkAllAsReadAsync(userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("No unread notifications found");
                }

                return ApiResponse<bool>.SuccessResponse(true, "All notifications marked as read");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while marking all notifications as read",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> DeleteNotificationAsync(int userId, int notificationId)
        {
            try
            {
                var success = await _notificationRepository.DeleteNotificationAsync(notificationId, userId);

                if (!success)
                {
                    return ApiResponse<bool>.ErrorResponse("Notification not found or unauthorized");
                }

                return ApiResponse<bool>.SuccessResponse(true, "Notification deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting notification {NotificationId}", notificationId);
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while deleting notification",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> SendNotificationToUserAsync(
            int adminId, SendNotificationToUserRequest request)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = request.UserId,
                    Title = request.Title,
                    Message = request.Message,
                    NotificationType = "ADMIN_ANNOUNCEMENT",
                    ReferenceId = request.ReferenceId,
                    ReferenceType = request.ReferenceType,
                    CreatedBy = adminId,
                    ImageUrl = request.ImageUrl,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _notificationRepository.CreateNotificationAsync(notification);

                return ApiResponse<bool>.SuccessResponse(true, "Notification sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification to user {UserId}", request.UserId);
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while sending notification",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<bool>> SendBulkNotificationAsync(
            int adminId, SendBulkNotificationRequest request)
        {
            try
            {
                List<int> targetUserIds;

                if (request.UserIds != null && request.UserIds.Any())
                {
                    targetUserIds = request.UserIds;
                }
                else
                {
                    targetUserIds = await _notificationRepository.GetAllUserIdsAsync();
                }

                var notifications = targetUserIds.Select(userId => new Notification
                {
                    UserId = userId,
                    Title = request.Title,
                    Message = request.Message,
                    NotificationType = "ADMIN_ANNOUNCEMENT",
                    CreatedBy = adminId,
                    ImageUrl = request.ImageUrl,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _notificationRepository.CreateBulkNotificationsAsync(notifications);

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    $"Notification sent to {targetUserIds.Count} users"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending bulk notification");
                return ApiResponse<bool>.ErrorResponse(
                    "An error occurred while sending bulk notification",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}