using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IFileUploadService
    {
        Task<UploadResponse> UploadImageAsync(IFormFile file, string folder = "posts");
        Task<UploadResponse> UploadVideoAsync(IFormFile file, string folder = "posts");
        Task<List<UploadResponse>> UploadMultipleImagesAsync(List<IFormFile> files, string folder = "posts");
        Task<bool> DeleteFileAsync(string filePath);
        string GenerateFileName(string originalFileName);
        bool IsValidImageFile(IFormFile file);
        bool IsValidVideoFile(IFormFile file);
    }
}
