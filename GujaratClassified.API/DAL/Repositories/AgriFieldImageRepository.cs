using Dapper;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using System.Data;

namespace GujaratClassified.API.DAL.Repositories
{
    public class AgriFieldImageRepository : IAgriFieldImageRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public AgriFieldImageRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<int> AddAgriFieldImageAsync(AgriFieldImage image)
        {
            using var connection = _connectionFactory.CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@AgriFieldId", image.AgriFieldId);
            parameters.Add("@ImageUrl", image.ImageUrl);
            parameters.Add("@ImagePath", image.ImagePath);
            parameters.Add("@IsMain", image.IsMain);
            parameters.Add("@SortOrder", image.SortOrder);
            parameters.Add("@Caption", image.Caption);
            parameters.Add("@ImageType", image.ImageType);
            parameters.Add("@OriginalFileName", image.OriginalFileName);
            parameters.Add("@FileSizeBytes", image.FileSizeBytes);
            parameters.Add("@MimeType", image.MimeType);

            var imageId = await connection.QuerySingleAsync<int>(
                "SP_AddAgriFieldImage",
                parameters,
                commandType: CommandType.StoredProcedure
            );


            return imageId;
        }

        //public async Task<List<AgriFieldImage>> GetAgriFieldImagesAsync(int agriFieldId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var images = await connection.QueryAsync<AgriFieldImage>(
        //        "SELECT * FROM AgriFieldImages WHERE AgriFieldId = @AgriFieldId ORDER BY SortOrder, CreatedAt",
        //        new { AgriFieldId = agriFieldId }
        //    );

        //    return images.ToList();
        //}
        public async Task<List<AgriFieldImage>> GetAgriFieldImagesAsync(int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var images = await connection.QueryAsync<AgriFieldImage>(
                "SP_GetAgriFieldImages",
                new { AgriFieldId = agriFieldId },
                commandType: CommandType.StoredProcedure
            );

            return images.ToList();
        }


        //public async Task<bool> DeleteAgriFieldImageAsync(int imageId, int agriFieldId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "DELETE FROM AgriFieldImages WHERE AgriImageId = @ImageId AND AgriFieldId = @AgriFieldId",
        //        new { ImageId = imageId, AgriFieldId = agriFieldId }
        //    );

        //    return result > 0;
        //}

        public async Task<bool> DeleteAgriFieldImageAsync(int imageId, int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rows = await connection.QuerySingleAsync<int>(
                "SP_DeleteAgriFieldImage",
                new { AgriImageId = imageId, AgriFieldId = agriFieldId },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        //public async Task<bool> SetMainImageAsync(int imageId, int agriFieldId)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    // First, unset all main images for this agri field
        //    await connection.ExecuteAsync(
        //        "UPDATE AgriFieldImages SET IsMain = 0 WHERE AgriFieldId = @AgriFieldId",
        //        new { AgriFieldId = agriFieldId }
        //    );

        //    // Then set the specified image as main
        //    var result = await connection.ExecuteAsync(
        //        "UPDATE AgriFieldImages SET IsMain = 1 WHERE AgriImageId = @ImageId AND AgriFieldId = @AgriFieldId",
        //        new { ImageId = imageId, AgriFieldId = agriFieldId }
        //    );

        //    return result > 0;
        //}

        public async Task<bool> SetMainImageAsync(int imageId, int agriFieldId)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rows = await connection.QuerySingleAsync<int>(
                "SP_SetMainAgriFieldImage",
                new { AgriImageId = imageId, AgriFieldId = agriFieldId },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

        //public async Task<bool> UpdateImageCaptionAsync(int imageId, string caption)
        //{
        //    using var connection = _connectionFactory.CreateConnection();

        //    var result = await connection.ExecuteAsync(
        //        "UPDATE AgriFieldImages SET Caption = @Caption WHERE AgriImageId = @ImageId",
        //        new { ImageId = imageId, Caption = caption }
        //    );

        //    return result > 0;
        //}

        public async Task<bool> UpdateImageCaptionAsync(int imageId, string caption)
        {
            using var connection = _connectionFactory.CreateConnection();

            var rows = await connection.QuerySingleAsync<int>(
                "SP_UpdateAgriFieldImageCaption",
                new { AgriImageId = imageId, Caption = caption },
                commandType: CommandType.StoredProcedure
            );

            return rows > 0;
        }

    }
}