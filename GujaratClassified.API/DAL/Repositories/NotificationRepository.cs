// DAL/Repositories/NotificationRepository.cs
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public NotificationRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> CreateNotificationAsync(Notification notification)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", notification.UserId);
            parameters.Add("@Title", notification.Title);
            parameters.Add("@Message", notification.Message);
            parameters.Add("@NotificationType", notification.NotificationType);
            parameters.Add("@ReferenceId", notification.ReferenceId);
            parameters.Add("@ReferenceType", notification.ReferenceType);
            parameters.Add("@CreatedBy", notification.CreatedBy);
            parameters.Add("@ImageUrl", notification.ImageUrl);

            var notificationId = await connection.QuerySingleAsync<int>(
                "SP_CreateNotification",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return notificationId;
        }

        //public async Task<bool> CreateBulkNotificationsAsync(List<Notification> notifications)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var sql = @"INSERT INTO Notifications 
        //               (UserId, Title, Message, NotificationType, ReferenceId, ReferenceType, CreatedBy, ImageUrl, CreatedAt)
        //               VALUES 
        //               (@UserId, @Title, @Message, @NotificationType, @ReferenceId, @ReferenceType, @CreatedBy, @ImageUrl, GETDATE())";

        //    var rowsAffected = await connection.ExecuteAsync(sql, notifications);
        //    return rowsAffected > 0;
        //}
        public async Task<bool> CreateBulkNotificationsAsync(List<Notification> notifications)
        {
            using var connection = _connectionFactory.CreateConnection();

            // Convert list to JSON without using directive
            string jsonData = System.Text.Json.JsonSerializer.Serialize(notifications);

            var parameters = new DynamicParameters();
            parameters.Add("@Notifications", jsonData);

            var rowsAffected = await connection.ExecuteAsync(
                "SP_CreateBulkNotifications",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }


        public async Task<Notification?> GetNotificationByIdAsync(int notificationId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@NotificationId", notificationId);

            var notification = await connection.QueryFirstOrDefaultAsync<Notification>(
                "SP_GetNotificationById",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return notification;
        }

        public async Task<(List<Notification> Notifications, int TotalCount)> GetUserNotificationsAsync(
            int userId, bool? isRead, int pageNumber, int pageSize)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);
            parameters.Add("@IsRead", isRead);
            parameters.Add("@PageNumber", pageNumber);
            parameters.Add("@PageSize", pageSize);

            using var multi = await connection.QueryMultipleAsync(
                "SP_GetUserNotifications",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            var notifications = (await multi.ReadAsync<Notification>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return (notifications, totalCount);
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var count = await connection.QuerySingleAsync<int>(
                "SP_GetUnreadNotificationCount",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return count;
        }

        public async Task<bool> MarkAsReadAsync(int notificationId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@NotificationId", notificationId);
            parameters.Add("@UserId", userId);

            var rowsAffected = await connection.ExecuteAsync(
                "SP_MarkNotificationAsRead",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<bool> MarkAllAsReadAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@UserId", userId);

            var rowsAffected = await connection.ExecuteAsync(
                "SP_MarkAllNotificationsAsRead",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@NotificationId", notificationId);
            parameters.Add("@UserId", userId);

            var rowsAffected = await connection.ExecuteAsync(
                "SP_DeleteNotification",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return rowsAffected > 0;
        }

        public async Task<List<int>> GetAllUserIdsAsync()
        {
            using var connection = _connectionFactory.CreateConnection();

            var userIds = await connection.QueryAsync<int>(
                "SP_GetAllActiveUserIds",
                commandType: CommandType.StoredProcedure
            );

            return userIds.ToList();
        }
    }
}