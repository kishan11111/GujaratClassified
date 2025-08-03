// Services/Implementations/UserPostService.cs
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.Services.Implementations
{
    public class UserPostService : IUserPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostImageRepository _imageRepository;
        private readonly IPostVideoRepository _videoRepository;
        private readonly IFavoriteRepository _favoriteRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IFileUploadService _fileUploadService;
        private readonly ILogger<UserPostService> _logger;

        public UserPostService(
            IPostRepository postRepository,
            IPostImageRepository imageRepository,
            IPostVideoRepository videoRepository,
            IFavoriteRepository favoriteRepository,
            ICategoryRepository categoryRepository,
            ILocationRepository locationRepository,
            IFileUploadService fileUploadService,
            ILogger<UserPostService> logger)
        {
            _postRepository = postRepository;
            _imageRepository = imageRepository;
            _videoRepository = videoRepository;
            _favoriteRepository = favoriteRepository;
            _categoryRepository = categoryRepository;
            _locationRepository = locationRepository;
            _fileUploadService = fileUploadService;
            _logger = logger;
        }

        public async Task<ApiResponse<PostResponse>> CreatePostAsync(int userId, CreatePostRequest request)
        {
            try
            {
                // Validate category and subcategory
                var category = await _categoryRepository.GetCategoryByIdAsync(request.CategoryId);
                if (category == null)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Invalid category selected");
                }

                var subCategory = await _categoryRepository.GetSubCategoryByIdAsync(request.SubCategoryId);
                if (subCategory == null || subCategory.CategoryId != request.CategoryId)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Invalid subcategory selected");
                }

                // Validate location
                var district = await _locationRepository.GetDistrictByIdAsync(request.DistrictId);
                if (district == null)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Invalid district selected");
                }

                var taluka = await _locationRepository.GetTalukaByIdAsync(request.TalukaId);
                if (taluka == null || taluka.DistrictId != request.DistrictId)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Invalid taluka selected");
                }

                var village = await _locationRepository.GetVillageByIdAsync(request.VillageId);
                if (village == null || village.TalukaId != request.TalukaId)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Invalid village selected");
                }

                // Create post entity
                var post = new Post
                {
                    UserId = userId,
                    CategoryId = request.CategoryId,
                    SubCategoryId = request.SubCategoryId,
                    Title = request.Title,
                    Description = request.Description,
                    Price = request.Price,
                    PriceType = request.PriceType,
                    Condition = request.Condition,
                    DistrictId = request.DistrictId,
                    TalukaId = request.TalukaId,
                    VillageId = request.VillageId,
                    Address = request.Address,
                    ContactMethod = request.ContactMethod,
                    ContactPhone = request.ContactPhone,
                    IsFeatured = request.IsFeatured,
                    Status = "ACTIVE",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Save post to database
                var postId = await _postRepository.CreatePostAsync(post);
                post.PostId = postId;

                // Get the created post with all details
                var createdPost = await _postRepository.GetPostByIdAsync(postId);
                var postResponse = MapPostToResponse(createdPost!);

                return ApiResponse<PostResponse>.SuccessResponse(postResponse, "Post created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating post for user {UserId}", userId);
                return ApiResponse<PostResponse>.ErrorResponse("An error occurred while creating the post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostResponse>> GetPostByIdAsync(int postId, int? viewerUserId = null)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Post not found");
                }

                // Increment view count (but don't count owner views)
                if (viewerUserId != post.UserId)
                {
                    await _postRepository.IncrementPostViewAsync(postId, viewerUserId, null, null);
                }

                var postResponse = MapPostToResponse(post);
                return ApiResponse<PostResponse>.SuccessResponse(postResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting post {PostId}", postId);
                return ApiResponse<PostResponse>.ErrorResponse("An error occurred while fetching the post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetMyPostsAsync(int userId, string? status, int pageSize, int pageNumber)
        {
            try
            {
                var (posts, totalCount) = await _postRepository.GetUserPostsAsync(userId, status, pageSize, pageNumber);

                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return ApiResponse<(List<PostListResponse>, PaginationResponse)>.SuccessResponse(
                    (postResponses, pagination),
                    $"Retrieved {postResponses.Count} posts successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting posts for user {UserId}", userId);
                return ApiResponse<(List<PostListResponse>, PaginationResponse)>.ErrorResponse(
                    "An error occurred while fetching posts", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostResponse>> UpdatePostAsync(int postId, int userId, UpdatePostRequest request)
        {
            try
            {
                var existingPost = await _postRepository.GetPostByIdAsync(postId);
                if (existingPost == null)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Post not found");
                }

                if (existingPost.UserId != userId)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("You can only update your own posts");
                }

                // Update post entity
                existingPost.Title = request.Title;
                existingPost.Description = request.Description;
                existingPost.Price = request.Price;
                existingPost.PriceType = request.PriceType;
                existingPost.Condition = request.Condition;
                existingPost.Address = request.Address;
                existingPost.ContactMethod = request.ContactMethod;
                existingPost.ContactPhone = request.ContactPhone;

                var updateResult = await _postRepository.UpdatePostAsync(existingPost);
                if (!updateResult)
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Failed to update post");
                }

                // Get updated post
                var updatedPost = await _postRepository.GetPostByIdAsync(postId);
                var postResponse = MapPostToResponse(updatedPost!);

                return ApiResponse<PostResponse>.SuccessResponse(postResponse, "Post updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating post {PostId} for user {UserId}", postId, userId);
                return ApiResponse<PostResponse>.ErrorResponse("An error occurred while updating the post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> UpdatePostStatusAsync(int postId, int userId, PostStatusRequest request)
        {
            try
            {
                var updateResult = await _postRepository.UpdatePostStatusAsync(postId, userId, request.Status);
                if (!updateResult)
                {
                    return ApiResponse<object>.ErrorResponse("Failed to update post status. Make sure you own this post.");
                }

                return ApiResponse<object>.SuccessResponse(null, $"Post status updated to {request.Status}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for post {PostId}", postId);
                return ApiResponse<object>.ErrorResponse("An error occurred while updating post status",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> DeletePostAsync(int postId, int userId)
        {
            try
            {
                var deleteResult = await _postRepository.DeletePostAsync(postId, userId);
                if (!deleteResult)
                {
                    return ApiResponse<object>.ErrorResponse("Failed to delete post. Make sure you own this post.");
                }

                return ApiResponse<object>.SuccessResponse(null, "Post deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting post {PostId} for user {UserId}", postId, userId);
                return ApiResponse<object>.ErrorResponse("An error occurred while deleting the post",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<List<UploadResponse>>> UploadPostImagesAsync(int postId, int userId, List<IFormFile> images)
        {
            try
            {
                // Verify post ownership
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null || post.UserId != userId)
                {
                    return ApiResponse<List<UploadResponse>>.ErrorResponse("Post not found or access denied");
                }

                var uploadResponses = new List<UploadResponse>();

                for (int i = 0; i < images.Count; i++)
                {
                    var uploadResult = await _fileUploadService.UploadImageAsync(images[i], "posts");

                    if (uploadResult.Success)
                    {
                        // Save image info to database
                        var postImage = new PostImage
                        {
                            PostId = postId,
                            ImageUrl = uploadResult.FileUrl!,
                            IsMain = i == 0 && (post.Images?.Count == 0), // First image as main if no images exist
                            SortOrder = i,
                            OriginalFileName = uploadResult.FileName,
                            FileSizeBytes = uploadResult.FileSizeBytes,
                            MimeType = uploadResult.MimeType
                        };

                        await _imageRepository.AddPostImageAsync(postImage);
                    }

                    uploadResponses.Add(uploadResult);
                }

                return ApiResponse<List<UploadResponse>>.SuccessResponse(uploadResponses,
                    $"Uploaded {uploadResponses.Count(r => r.Success)} of {images.Count} images");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading images for post {PostId}", postId);
                return ApiResponse<List<UploadResponse>>.ErrorResponse("An error occurred while uploading images",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<UploadResponse>> UploadPostVideoAsync(int postId, int userId, IFormFile video)
        {
            try
            {
                // Verify post ownership
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null || post.UserId != userId)
                {
                    return ApiResponse<UploadResponse>.ErrorResponse("Post not found or access denied");
                }

                var uploadResult = await _fileUploadService.UploadVideoAsync(video, "posts");

                if (uploadResult.Success)
                {
                    // Save video info to database
                    var postVideo = new PostVideo
                    {
                        PostId = postId,
                        VideoUrl = uploadResult.FileUrl!,
                        SortOrder = 0,
                        OriginalFileName = uploadResult.FileName,
                        FileSizeBytes = uploadResult.FileSizeBytes,
                        MimeType = uploadResult.MimeType
                    };

                    await _videoRepository.AddPostVideoAsync(postVideo);
                }

                return ApiResponse<UploadResponse>.SuccessResponse(uploadResult,
                    uploadResult.Success ? "Video uploaded successfully" : "Failed to upload video");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video for post {PostId}", postId);
                return ApiResponse<UploadResponse>.ErrorResponse("An error occurred while uploading video",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<(List<PostListResponse> Posts, PaginationResponse Pagination)>> GetUserFavoritesAsync(int userId, int pageSize, int pageNumber)
        {
            try
            {
                var (posts, totalCount) = await _postRepository.GetUserFavoritesAsync(userId, pageSize, pageNumber);

                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize)
                };

                return ApiResponse<(List<PostListResponse>, PaginationResponse)>.SuccessResponse(
                    (postResponses, pagination),
                    $"Retrieved {postResponses.Count} favorite posts successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting favorites for user {UserId}", userId);
                return ApiResponse<(List<PostListResponse>, PaginationResponse)>.ErrorResponse(
                    "An error occurred while fetching favorites", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<string>> ToggleFavoriteAsync(int userId, int postId)
        {
            try
            {
                var action = await _favoriteRepository.ToggleFavoriteAsync(userId, postId);
                var message = action == "ADDED" ? "Added to favorites" : "Removed from favorites";

                return ApiResponse<string>.SuccessResponse(action, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling favorite for user {UserId} and post {PostId}", userId, postId);
                return ApiResponse<string>.ErrorResponse("An error occurred while updating favorites",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostStatsResponse>> GetUserPostStatsAsync(int userId)
        {
            try
            {
                var stats = await _postRepository.GetUserPostStatsAsync(userId);
                return ApiResponse<PostStatsResponse>.SuccessResponse(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting stats for user {UserId}", userId);
                return ApiResponse<PostStatsResponse>.ErrorResponse("An error occurred while fetching statistics",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> ContactSellerAsync(int userId, ContactSellerRequest request)
        {
            try
            {
                // Increment contact count
                await _postRepository.IncrementContactCountAsync(request.PostId);

                // TODO: Implement actual contact/messaging system
                // For now, just log the contact attempt
                _logger.LogInformation("User {UserId} contacted seller for post {PostId}: {Message}",
                    userId, request.PostId, request.Message);

                return ApiResponse<object>.SuccessResponse(null, "Contact request sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact request from user {UserId}", userId);
                return ApiResponse<object>.ErrorResponse("An error occurred while sending contact request",
                    new List<string> { ex.Message });
            }
        }

        // Private helper methods
        private PostResponse MapPostToResponse(Post post)
        {
            return new PostResponse
            {
                PostId = post.PostId,
                UserId = post.UserId,
                CategoryId = post.CategoryId,
                SubCategoryId = post.SubCategoryId,
                Title = post.Title,
                Description = post.Description,
                Price = post.Price,
                PriceType = post.PriceType,
                Condition = post.Condition,
                DistrictId = post.DistrictId,
                TalukaId = post.TalukaId,
                VillageId = post.VillageId,
                Address = post.Address,
                ContactMethod = post.ContactMethod,
                ContactPhone = post.ContactPhone,
                IsActive = post.IsActive,
                IsSold = post.IsSold,
                IsFeatured = post.IsFeatured,
                Status = post.Status,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                SoldAt = post.SoldAt,
                ExpiryDate = post.ExpiryDate,
                ViewCount = post.ViewCount,
                ContactCount = post.ContactCount,
                FavoriteCount = post.FavoriteCount,
                UserName = post.UserName,
                UserMobile = post.UserMobile,
                UserVerified = post.UserVerified,
                CategoryName = post.CategoryName,
                SubCategoryName = post.SubCategoryName,
                DistrictName = post.DistrictName,
                TalukaName = post.TalukaName,
                VillageName = post.VillageName,
                Images = post.Images?.Select(i => new PostImageResponse
                {
                    ImageId = i.ImageId,
                    PostId = i.PostId,
                    ImageUrl = i.ImageUrl,
                    IsMain = i.IsMain,
                    SortOrder = i.SortOrder,
                    CreatedAt = i.CreatedAt,
                    OriginalFileName = i.OriginalFileName,
                    FileSizeBytes = i.FileSizeBytes,
                    MimeType = i.MimeType
                }).ToList(),
                Videos = post.Videos?.Select(v => new PostVideoResponse
                {
                    VideoId = v.VideoId,
                    PostId = v.PostId,
                    VideoUrl = v.VideoUrl,
                    ThumbnailUrl = v.ThumbnailUrl,
                    SortOrder = v.SortOrder,
                    CreatedAt = v.CreatedAt,
                    OriginalFileName = v.OriginalFileName,
                    FileSizeBytes = v.FileSizeBytes,
                    MimeType = v.MimeType,
                    DurationSeconds = v.DurationSeconds
                }).ToList()
            };
        }

        private PostListResponse MapPostToListResponse(Post post)
        {
            return new PostListResponse
            {
                PostId = post.PostId,
                Title = post.Title,
                Price = post.Price,
                PriceType = post.PriceType,
                Condition = post.Condition,
                Status = post.Status,
                IsFeatured = post.IsFeatured,
                CreatedAt = post.CreatedAt,
                ViewCount = post.ViewCount,
                FavoriteCount = post.FavoriteCount,
                DistrictName = post.DistrictName,
                TalukaName = post.TalukaName,
                VillageName = post.VillageName,
                CategoryName = post.CategoryName,
                SubCategoryName = post.SubCategoryName,
                MainImageUrl = post.Images?.FirstOrDefault(i => i.IsMain)?.ImageUrl ??
                              post.Images?.FirstOrDefault()?.ImageUrl
            };
        }
    }
}