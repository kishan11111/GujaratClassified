
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

        public async Task<List<PostImage>> GetPostImagesAsync(int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var images = await connection.QueryAsync<PostImage>(
                "SELECT * FROM PostImages WHERE PostId = @PostId ORDER BY SortOrder, CreatedAt",
                new { PostId = postId }
            );

            return images.ToList();
        }

        public async Task<bool> DeletePostImageAsync(int imageId, int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "DELETE FROM PostImages WHERE ImageId = @ImageId AND PostId = @PostId",
                new { ImageId = imageId, PostId = postId }
            );

            return result > 0;
        }

        public async Task<bool> SetMainImageAsync(int imageId, int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            // First, unset all main images for this post
            await connection.ExecuteAsync(
                "UPDATE PostImages SET IsMain = 0 WHERE PostId = @PostId",
                new { PostId = postId }
            );

            // Then set the specified image as main
            var result = await connection.ExecuteAsync(
                "UPDATE PostImages SET IsMain = 1 WHERE ImageId = @ImageId AND PostId = @PostId",
                new { ImageId = imageId, PostId = postId }
            );

            return result > 0;
        }
    }
}