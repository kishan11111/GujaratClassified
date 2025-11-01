// Controllers/LocalCardFileUploadController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/localcard/upload")]
    [Produces("application/json")]
    //[Authorize]
    public class LocalCardFileUploadController : ControllerBase
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<LocalCardFileUploadController> _logger;

        public LocalCardFileUploadController(
            IFileUploadService fileUploadService,
            ILogger<LocalCardFileUploadController> logger)
        {
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        /// <summary>
        /// Upload Profile Image (સિંગલ ઈમેજ)
        /// </summary>
        /// <param name="file">Profile image file</param>
        /// <returns>Uploaded image URL</returns>
        [HttpPost("profile-image")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { success = false, message = "Only JPG, PNG, and WEBP images are allowed" });
            }

            // Validate file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { success = false, message = "File size cannot exceed 5MB" });
            }

            try
            {
                var result = await _fileUploadService.UploadImageAsync(file, "localcards/profiles");

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Profile image uploaded successfully",
                        data = new
                        {
                            imageUrl = result.FileUrl,
                            fileName = result.FileName,
                            fileSize = result.FileSizeBytes
                        }
                    });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading profile image");
                return StatusCode(500, new { success = false, message = "Error uploading file" });
            }
        }

        /// <summary>
        /// Upload Cover Image (સિંગલ ઈમેજ)
        /// </summary>
        /// <param name="file">Cover image file</param>
        /// <returns>Uploaded image URL</returns>
        [HttpPost("cover-image")]
        public async Task<IActionResult> UploadCoverImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded" });
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { success = false, message = "Only JPG, PNG, and WEBP images are allowed" });
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { success = false, message = "File size cannot exceed 5MB" });
            }

            try
            {
                var result = await _fileUploadService.UploadImageAsync(file, "localcards/covers");

                if (result.Success)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Cover image uploaded successfully",
                        data = new
                        {
                            imageUrl = result.FileUrl,
                            fileName = result.FileName,
                            fileSize = result.FileSizeBytes
                        }
                    });
                }

                return BadRequest(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading cover image");
                return StatusCode(500, new { success = false, message = "Error uploading file" });
            }
        }

        /// <summary>
        /// Upload Multiple Additional Images (મલ્ટીપલ ઈમેજ - max 10)
        /// </summary>
        /// <param name="files">Multiple image files</param>
        /// <returns>List of uploaded image URLs</returns>
        //[HttpPost("additional-images")]
        //public async Task<IActionResult> UploadAdditionalImages(List<IFormFile> files)
        //{
        //    if (files == null || files.Count == 0)
        //    {
        //        return BadRequest(new { success = false, message = "No files uploaded" });
        //    }

        //    // Max 10 images
        //    if (files.Count > 10)
        //    {
        //        return BadRequest(new { success = false, message = "Maximum 10 images allowed" });
        //    }

        //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        //    var uploadedImages = new List<object>();
        //    var errors = new List<string>();

        //    foreach (var file in files)
        //    {
        //        // Validate each file
        //        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        //        if (!allowedExtensions.Contains(extension))
        //        {
        //            errors.Add($"{file.FileName}: Only JPG, PNG, and WEBP images are allowed");
        //            continue;
        //        }

        //        if (file.Length > 5 * 1024 * 1024)
        //        {
        //            errors.Add($"{file.FileName}: File size cannot exceed 5MB");
        //            continue;
        //        }

        //        try
        //        {
        //            var result = await _fileUploadService.UploadImageAsync(file, "localcards/gallery");

        //            if (result.Success)
        //            {
        //                uploadedImages.Add(new
        //                {
        //                    imageUrl = result.FileUrl,
        //                    fileName = result.FileName,
        //                    fileSize = result.FileSizeBytes
        //                });
        //            }
        //            else
        //            {
        //                errors.Add($"{file.FileName}: {result.Message}");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error uploading file {FileName}", file.FileName);
        //            errors.Add($"{file.FileName}: Upload failed");
        //        }
        //    }

        //    if (uploadedImages.Count == 0)
        //    {
        //        return BadRequest(new
        //        {
        //            success = false,
        //            message = "No images uploaded successfully",
        //            errors = errors
        //        });
        //    }

        //    return Ok(new
        //    {
        //        success = true,
        //        message = $"{uploadedImages.Count} image(s) uploaded successfully",
        //        data = new
        //        {
        //            uploadedImages = uploadedImages,
        //            totalUploaded = uploadedImages.Count,
        //            totalFailed = errors.Count
        //        },
        //        errors = errors.Count > 0 ? errors : null
        //    });
        //}


        [HttpPost("additional-images")]
        public async Task<IActionResult> UploadAdditionalImages(List<IFormFile> files)
        {
            try
            {
                if (files == null || files.Count == 0)
                    return BadRequest(new { success = false, message = "No files uploaded" });

                if (files.Count > 10)
                    return BadRequest(new { success = false, message = "Maximum 10 images allowed" });

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var uploadedImages = new List<object>();
                var errors = new List<string>();

                foreach (var file in files)
                {
                    try
                    {
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                        {
                            errors.Add($"{file.FileName}: Only JPG, PNG, and WEBP images are allowed");
                            continue;
                        }

                        if (file.Length > 5 * 1024 * 1024)
                        {
                            errors.Add($"{file.FileName}: File size cannot exceed 5MB");
                            continue;
                        }

                        var result = await _fileUploadService.UploadImageAsync(file, "localcards/gallery");

                        if (result.Success)
                        {
                            uploadedImages.Add(new
                            {
                                imageUrl = result.FileUrl,
                                fileName = result.FileName,
                                fileSize = result.FileSizeBytes
                            });
                        }
                        else
                        {
                            errors.Add($"{file.FileName}: {result.Message}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error uploading file {FileName}", file.FileName);
                        errors.Add($"{file.FileName}: Upload failed ({ex.Message})");
                    }
                }

                if (uploadedImages.Count == 0)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "No images uploaded successfully",
                        errors
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = $"{uploadedImages.Count} image(s) uploaded successfully",
                    data = new
                    {
                        uploadedImages,
                        totalUploaded = uploadedImages.Count,
                        totalFailed = errors.Count
                    },
                    errors = errors.Count > 0 ? errors : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in UploadAdditionalImages endpoint");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Internal server error occurred",
                    error = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }


        /// <summary>
        /// Upload All Images at Once (પ્રોફાઈલ + કવર + વધારાના)
        /// </summary>
        /// <param name="profileImage">Profile image (optional)</param>
        /// <param name="coverImage">Cover image (optional)</param>
        /// <param name="additionalImages">Additional images (optional, max 10)</param>
        /// <returns>All uploaded image URLs</returns>
        [HttpPost("all-images")]
        public async Task<IActionResult> UploadAllImages(
            IFormFile? profileImage,
            IFormFile? coverImage,
            List<IFormFile>? additionalImages)
        {
            var response = new
            {
                profileImageUrl = (string?)null,
                coverImageUrl = (string?)null,
                additionalImageUrls = new List<string>()
            };

            var errors = new List<string>();

            try
            {
                // Upload profile image
                if (profileImage != null && profileImage.Length > 0)
                {
                    var profileResult = await _fileUploadService.UploadImageAsync(profileImage, "localcards/profiles");
                    if (profileResult.Success)
                    {
                        response = response with { profileImageUrl = profileResult.FileUrl };
                    }
                    else
                    {
                        errors.Add($"Profile image: {profileResult.Message}");
                    }
                }

                // Upload cover image
                if (coverImage != null && coverImage.Length > 0)
                {
                    var coverResult = await _fileUploadService.UploadImageAsync(coverImage, "localcards/covers");
                    if (coverResult.Success)
                    {
                        response = response with { coverImageUrl = coverResult.FileUrl };
                    }
                    else
                    {
                        errors.Add($"Cover image: {coverResult.Message}");
                    }
                }

                // Upload additional images
                if (additionalImages != null && additionalImages.Count > 0)
                {
                    var additionalUrls = new List<string>();

                    foreach (var file in additionalImages.Take(10)) // Max 10
                    {
                        var result = await _fileUploadService.UploadImageAsync(file, "localcards/gallery");
                        if (result.Success)
                        {
                            additionalUrls.Add(result.FileUrl);
                        }
                        else
                        {
                            errors.Add($"{file.FileName}: {result.Message}");
                        }
                    }

                    response = response with { additionalImageUrls = additionalUrls };
                }

                return Ok(new
                {
                    success = true,
                    message = "Images uploaded successfully",
                    data = response,
                    errors = errors.Count > 0 ? errors : null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading images");
                return StatusCode(500, new { success = false, message = "Error uploading images" });
            }
        }
    }
}