// DAL/Repositories/PostVideoRepository.cs
using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class PostVideoRepository : IPostVideoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public PostVideoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddPostVideoAsync(PostVideo video)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@PostId", video.PostId);
            parameters.Add("@VideoUrl", video.VideoUrl);
            parameters.Add("@VideoPath", video.VideoPath);
            parameters.Add("@ThumbnailUrl", video.ThumbnailUrl);
            parameters.Add("@SortOrder", video.SortOrder);
            parameters.Add("@OriginalFileName", video.OriginalFileName);
            parameters.Add("@FileSizeBytes", video.FileSizeBytes);
            parameters.Add("@MimeType", video.MimeType);
            parameters.Add("@DurationSeconds", video.DurationSeconds);

            var videoId = await connection.QuerySingleAsync<int>(
                "SP_AddPostVideo",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return videoId;
        }

        public async Task<List<PostVideo>> GetPostVideosAsync(int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var videos = await connection.QueryAsync<PostVideo>(
                "SELECT * FROM PostVideos WHERE PostId = @PostId ORDER BY SortOrder, CreatedAt",
                new { PostId = postId }
            );

            return videos.ToList();
        }

        public async Task<bool> DeletePostVideoAsync(int videoId, int postId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "DELETE FROM PostVideos WHERE VideoId = @VideoId AND PostId = @PostId",
                new { VideoId = videoId, PostId = postId }
            );

            return result > 0;
        }
    }
}