
// DAL/Repositories/PostImageRepository.cs
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class PostImageRepository : IPostImageRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PostImageRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddPostImageAsync(PostImage image)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", image.PostId);
            parameters.Add("@ImageUrl", image.ImageUrl);
            parameters.Add("@ImagePath", image.ImagePath);
            parameters.Add("@IsMain", image.IsMain);
            parameters.Add("@SortOrder", image.SortOrder);
            parameters.Add("@OriginalFileName", image.OriginalFileName);
            parameters.Add("@FileSizeBytes", image.FileSizeBytes);
            parameters.Add("@MimeType", image.MimeType);

            var imageId = await connection.QuerySingleAsync<int>(
                "SP_AddPostImage",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return imageId;
        }

        //public async Task LogSystemInfoAsync(string procedureName, int? postId, string logType, string logMessage)
        //{
        //    try
        //    {
        //        using var connection = _connectionFactory.CreateConnection();
        //        var parameters = new DynamicParameters();
        //        parameters.Add("@ProcedureName", procedureName);
        //        parameters.Add("@PostId", postId);
        //        parameters.Add("@LogType", logType); // e.g., INFO, DEBUG, SUCCESS
        //        parameters.Add("@LogMessage", logMessage);

        //        await connection.ExecuteAsync(@"
        //    INSERT INTO SystemLogs (ProcedureName, PostId, LogType, LogMessage)
        //    VALUES (@ProcedureName, @PostId, @LogType, @LogMessage)
        //", parameters);
        //    }
        //    catch
        //    {
        //        // Prevent recursive logging failure
        //    }
        //}
        public async Task LogSystemInfoAsync(string procedureName, int? postId, string logType, string logMessage)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();

                var parameters = new DynamicParameters();
                parameters.Add("@ProcedureName", procedureName);
                parameters.Add("@PostId", postId);
                parameters.Add("@LogType", logType);
                parameters.Add("@LogMessage", logMessage);

                await connection.ExecuteAsync(
                    "SP_LogSystemInfo",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch
            {
                // Prevent recursive logging failure
            }
        }

        public async Task LogSystemErrorAsync(string methodName, string procedureName, int? postId, int? userId, string errorMessage, string stackTrace)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var parameters = new DynamicParameters();
                parameters.Add("@MethodName", methodName);
                parameters.Add("@ProcedureName", procedureName);
                parameters.Add("@PostId", postId);
                parameters.Add("@UserId", userId);
                parameters.Add("@ErrorMessage", errorMessage);
                parameters.Add("@StackTrace", stackTrace);

                await connection.ExecuteAsync("SP_InsertSystemErrorLog", parameters, commandType: CommandType.StoredProcedure);
            }
            catch
            {
                // avoid recursion if logging fails
            }
        }

        //public async Task<List<PostImage>> GetPostImagesAsync(int postId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var images = await connection.QueryAsync<PostImage>(
        //        "SELECT * FROM PostImages WHERE PostId = @PostId ORDER BY SortOrder, CreatedAt",
        //        new { PostId = postId }
        //    );

        //    return images.ToList();
        //}
        public async Task<List<PostImage>> GetPostImagesAsync(int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", postId);

            var images = await connection.QueryAsync<PostImage>(
                "SP_GetPostImages",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return images.ToList();
        }

        //public async Task<bool> DeletePostImageAsync(int imageId, int postId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "DELETE FROM PostImages WHERE ImageId = @ImageId AND PostId = @PostId",
        //        new { ImageId = imageId, PostId = postId }
        //    );

        //    return result > 0;
        //}

        public async Task<bool> DeletePostImageAsync(int imageId, int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ImageId", imageId);
            parameters.Add("@PostId", postId);

            var result = await connection.ExecuteAsync(
                "SP_DeletePostImage",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

        //public async Task<bool> SetMainImageAsync(int imageId, int postId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    // First, unset all main images for this post
        //    await connection.ExecuteAsync(
        //        "UPDATE PostImages SET IsMain = 0 WHERE PostId = @PostId",
        //        new { PostId = postId }
        //    );

        //    // Then set the specified image as main
        //    var result = await connection.ExecuteAsync(
        //        "UPDATE PostImages SET IsMain = 1 WHERE ImageId = @ImageId AND PostId = @PostId",
        //        new { ImageId = imageId, PostId = postId }
        //    );

        //    return result > 0;
        //}
        public async Task<bool> SetMainImageAsync(int imageId, int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@ImageId", imageId);
            parameters.Add("@PostId", postId);

            var result = await connection.ExecuteAsync(
                "SP_SetMainImage",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return result > 0;
        }

    }
}