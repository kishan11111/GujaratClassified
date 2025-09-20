using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AgriFieldVideoRepository : IAgriFieldVideoRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AgriFieldVideoRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddAgriFieldVideoAsync(AgriFieldVideo video)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AgriFieldId", video.AgriFieldId);
            parameters.Add("@VideoUrl", video.VideoUrl);
            parameters.Add("@VideoPath", video.VideoPath);
            parameters.Add("@ThumbnailUrl", video.ThumbnailUrl);
            parameters.Add("@SortOrder", video.SortOrder);
            parameters.Add("@Caption", video.Caption);
            parameters.Add("@VideoType", video.VideoType);
            parameters.Add("@OriginalFileName", video.OriginalFileName);
            parameters.Add("@FileSizeBytes", video.FileSizeBytes);
            parameters.Add("@MimeType", video.MimeType);
            parameters.Add("@DurationSeconds", video.DurationSeconds);

            var videoId = await connection.QuerySingleAsync<int>(
                "SP_AddAgriFieldVideo",
                parameters,
                commandType: CommandType.StoredProcedure
            );

            return videoId;
        }

        public async Task<List<AgriFieldVideo>> GetAgriFieldVideosAsync(int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var videos = await connection.QueryAsync<AgriFieldVideo>(
                "SELECT * FROM AgriFieldVideos WHERE AgriFieldId = @AgriFieldId ORDER BY SortOrder, CreatedAt",
                new { AgriFieldId = agriFieldId }
            );

            return videos.ToList();
        }

        public async Task<bool> DeleteAgriFieldVideoAsync(int videoId, int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "DELETE FROM AgriFieldVideos WHERE AgriVideoId = @VideoId AND AgriFieldId = @AgriFieldId",
                new { VideoId = videoId, AgriFieldId = agriFieldId }
            );

            return result > 0;
        }

        public async Task<bool> UpdateVideoCaptionAsync(int videoId, string caption)
        {
            using var connection = _connectionFactory.CreateConnection();

            var result = await connection.ExecuteAsync(
                "UPDATE AgriFieldVideos SET Caption = @Caption WHERE AgriVideoId = @VideoId",
                new { VideoId = videoId, Caption = caption }
            );

            return result > 0;
        }
    }
}