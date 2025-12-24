using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.Models.Response;
using Microsoft.AspNetCore.WebUtilities;
using static System.Net.Mime.MediaTypeNames;
using ImageSharpImage = SixLabors.ImageSharp.Image;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp;

namespace GujaratClassified.API.Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private readonly string[] _allowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".flv" };
        private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB
        private const long MaxVideoSizeBytes = 50 * 1024 * 1024; // 50 MB

        public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        //public async Task<UploadResponse> UploadImageAsync(IFormFile file, string folder = "posts")
        //{
        //    try
        //    {
        //        // Validate file
        //        if (!IsValidImageFile(file))
        //        {
        //            return new UploadResponse
        //            {
        //                Success = false,
        //                Message = "Invalid image file",
        //                Errors = new List<string> { "Only JPG, PNG, GIF, WEBP files are allowed" }
        //            };
        //        }

        //        // Generate unique filename
        //        var fileName = GenerateFileName(file.FileName);
        //        var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folder);

        //        // Create directory if it doesn't exist
        //        if (!Directory.Exists(uploadsPath))
        //        {
        //            Directory.CreateDirectory(uploadsPath);
        //        }

        //        var filePath = Path.Combine(uploadsPath, fileName);
        //        var fileUrl = $"/uploads/{folder}/{fileName}";

        //        // Save file
        //        using (var stream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await file.CopyToAsync(stream);
        //        }

        //        _logger.LogInformation("Image uploaded successfully: {FileName}", fileName);

        //        return new UploadResponse
        //        {
        //            Success = true,
        //            Message = "Image uploaded successfully",
        //            FileUrl = fileUrl,
        //            FileName = fileName,
        //            FileSizeBytes = file.Length,
        //            MimeType = file.ContentType
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);
        //        return new UploadResponse
        //        {
        //            Success = false,
        //            Message = "Error uploading image",
        //            Errors = new List<string> { ex.Message }
        //        };
        //    }
        //}

        public async Task<UploadResponse> UploadImageAsync(IFormFile file, string folder = "posts")
        {
            try
            {
                if (!IsValidImageFile(file))
                {
                    return new UploadResponse
                    {
                        Success = false,
                        Message = "Invalid image file",
                        Errors = new List<string> { "Only JPG, PNG, GIF, WEBP files are allowed" }
                    };
                }

                // Generate filename + paths
                var fileName = GenerateFileName(file.FileName);
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folder);

                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                var filePath = Path.Combine(uploadsPath, fileName);
                var fileUrl = $"/uploads/{folder}/{fileName}";

                // 🔥 Load image with ImageSharp
                using var image = await ImageSharpImage.LoadAsync(file.OpenReadStream());

                // 🔥 Resize if width is large (max 1080px)
                int maxWidth = 1080;
                if (image.Width > maxWidth)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Mode = ResizeMode.Max,
                        Size = new Size(maxWidth, 0)
                    }));
                }

                // 🔥 WebP compression settings
                var encoder = new WebpEncoder
                {
                    Quality = 75,
                    FileFormat = WebpFileFormatType.Lossy
                };

                // 🔥 Save optimized image
                await image.SaveAsync(filePath, encoder);

                _logger.LogInformation("Optimized image saved successfully: {FileName}", fileName);

                return new UploadResponse
                {
                    Success = true,
                    Message = "Image uploaded successfully",
                    FileUrl = fileUrl,
                    FileName = fileName,
                    FileSizeBytes = new FileInfo(filePath).Length,
                    MimeType = file.ContentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", file.FileName);
                return new UploadResponse
                {
                    Success = false,
                    Message = "Error uploading image",
                    Errors = new List<string> { ex.Message }
                };
            }
        }


        public async Task<UploadResponse> UploadVideoAsync(IFormFile file, string folder = "posts")
        {
            try
            {
                // Validate file
                if (!IsValidVideoFile(file))
                {
                    return new UploadResponse
                    {
                        Success = false,
                        Message = "Invalid video file",
                        Errors = new List<string> { "Only MP4, AVI, MOV, WMV, FLV files are allowed" }
                    };
                }

                // Generate unique filename
                var fileName = GenerateFileName(file.FileName);
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads", folder, "videos");

                // Create directory if it doesn't exist
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                var filePath = Path.Combine(uploadsPath, fileName);
                var fileUrl = $"/uploads/{folder}/videos/{fileName}";

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                _logger.LogInformation("Video uploaded successfully: {FileName}", fileName);

                return new UploadResponse
                {
                    Success = true,
                    Message = "Video uploaded successfully",
                    FileUrl = fileUrl,
                    FileName = fileName,
                    FileSizeBytes = file.Length,
                    MimeType = file.ContentType
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video: {FileName}", file.FileName);
                return new UploadResponse
                {
                    Success = false,
                    Message = "Error uploading video",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<List<UploadResponse>> UploadMultipleImagesAsync(List<IFormFile> files, string folder = "posts")
        {
            var responses = new List<UploadResponse>();

            foreach (var file in files)
            {
                var response = await UploadImageAsync(file, folder);
                responses.Add(response);
            }

            return responses;
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    _logger.LogInformation("File deleted successfully: {FilePath}", filePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file: {FilePath}", filePath);
                return false;
            }
        }

        public string GenerateFileName(string originalFileName)
        {
            var extension = Path.GetExtension(originalFileName);
            var uniqueName = $"{Guid.NewGuid()}{extension}";
            return uniqueName;
        }

        public bool IsValidImageFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxImageSizeBytes)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedImageExtensions.Contains(extension))
                return false;

            // Check MIME type
            var allowedMimeTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }

        public bool IsValidVideoFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            if (file.Length > MaxVideoSizeBytes)
                return false;

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedVideoExtensions.Contains(extension))
                return false;

            // Check MIME type
            var allowedMimeTypes = new[] { "video/mp4", "video/avi", "video/quicktime", "video/x-ms-wmv", "video/x-flv" };
            if (!allowedMimeTypes.Contains(file.ContentType.ToLowerInvariant()))
                return false;

            return true;
        }
    }
}