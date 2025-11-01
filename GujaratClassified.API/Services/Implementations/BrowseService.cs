// Services/Implementations/BrowseService.cs
using GujaratClassified.API.Services.Interfaces;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.Services.Implementations
{
    public class BrowseService : IBrowseService
    {
        private readonly IPostRepository _postRepository;
        private readonly ILogger<BrowseService> _logger;

        public BrowseService(IPostRepository postRepository, ILogger<BrowseService> logger)
        {
            _postRepository = postRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<PostListWithPaginationResponse>> GetAllPostsAsync(PostFilterRequest filter)
        {
            try
            {
                // Ensure we only show active posts
                filter.Status = "ACTIVE";

                var (posts, totalCount) = await _postRepository.GetPostsWithFiltersAsync(filter);
                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                var response = new PostListWithPaginationResponse
                {
                    Items = postResponses,
                    Pagination = pagination
                };

                return ApiResponse<PostListWithPaginationResponse>.SuccessResponse(
                    response,
                    $"Retrieved {postResponses.Count} posts successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all posts with filters");
                return ApiResponse<PostListWithPaginationResponse>.ErrorResponse(
                    "An error occurred while fetching posts", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostListWithPaginationResponse>> GetPostsByCategoryAsync(int categoryId, PostFilterRequest filter)
        {
            try
            {
                // Set category filter and ensure we only show active posts
                filter.CategoryId = categoryId;
                filter.Status = "ACTIVE";

                var (posts, totalCount) = await _postRepository.GetPostsWithFiltersAsync(filter);
                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                var response = new PostListWithPaginationResponse
                {
                    Items = postResponses,
                    Pagination = pagination
                };

                return ApiResponse<PostListWithPaginationResponse>.SuccessResponse(
                    response,
                    $"Retrieved {postResponses.Count} posts from category successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting posts by category {CategoryId}", categoryId);
                return ApiResponse<PostListWithPaginationResponse>.ErrorResponse(
                    "An error occurred while fetching posts by category", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostListWithPaginationResponse>> SearchPostsAsync(string searchTerm, PostFilterRequest filter)
        {
            try
            {
                // Set search term and ensure we only show active posts
                filter.SearchTerm = searchTerm;
                filter.Status = "ACTIVE";

                var (posts, totalCount) = await _postRepository.GetPostsWithFiltersAsync(filter);
                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                var response = new PostListWithPaginationResponse
                {
                    Items = postResponses,
                    Pagination = pagination
                };

                return ApiResponse<PostListWithPaginationResponse>.SuccessResponse(
                    response,
                    $"Found {postResponses.Count} posts matching '{searchTerm}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching posts with term {SearchTerm}", searchTerm);
                return ApiResponse<PostListWithPaginationResponse>.ErrorResponse(
                    "An error occurred while searching posts", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostListWithPaginationResponse>> GetPostsByLocationAsync(int districtId, PostFilterRequest filter)
        {
            try
            {
                // Set location filter and ensure we only show active posts
                filter.DistrictId = districtId;
                filter.Status = "ACTIVE";

                var (posts, totalCount) = await _postRepository.GetPostsWithFiltersAsync(filter);
                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                var response = new PostListWithPaginationResponse
                {
                    Items = postResponses,
                    Pagination = pagination
                };

                return ApiResponse<PostListWithPaginationResponse>.SuccessResponse(
                    response,
                    $"Retrieved {postResponses.Count} posts from location successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting posts by location {DistrictId}", districtId);
                return ApiResponse<PostListWithPaginationResponse>.ErrorResponse(
                    "An error occurred while fetching posts by location", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostListWithPaginationResponse>> GetFeaturedPostsAsync(PostFilterRequest filter)
        {
            try
            {
                // Set featured filter and ensure we only show active posts
                filter.IsFeatured = true;
                filter.Status = "ACTIVE";
                filter.SortBy = "NEWEST"; // Featured posts sorted by newest first

                var (posts, totalCount) = await _postRepository.GetPostsWithFiltersAsync(filter);
                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                var response = new PostListWithPaginationResponse
                {
                    Items = postResponses,
                    Pagination = pagination
                };

                return ApiResponse<PostListWithPaginationResponse>.SuccessResponse(
                    response,
                    $"Retrieved {postResponses.Count} featured posts successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting featured posts");
                return ApiResponse<PostListWithPaginationResponse>.ErrorResponse(
                    "An error occurred while fetching featured posts", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostListWithPaginationResponse>> GetLatestPostsAsync(PostFilterRequest filter)
        {
            try
            {
                // Set latest filter and ensure we only show active posts
                filter.Status = "ACTIVE";
                filter.SortBy = "NEWEST"; // Latest posts sorted by newest first

                var (posts, totalCount) = await _postRepository.GetPostsWithFiltersAsync(filter);
                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                var response = new PostListWithPaginationResponse
                {
                    Items = postResponses,
                    Pagination = pagination
                };

                return ApiResponse<PostListWithPaginationResponse>.SuccessResponse(
                    response,
                    $"Retrieved {postResponses.Count} latest posts successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting latest posts");
                return ApiResponse<PostListWithPaginationResponse>.ErrorResponse(
                    "An error occurred while fetching latest posts", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostListWithPaginationResponse>> GetPopularPostsAsync(PostFilterRequest filter)
        {
            try
            {
                // Set popular filter and ensure we only show active posts
                filter.Status = "ACTIVE";
                filter.SortBy = "POPULAR"; // Popular posts sorted by view count

                var (posts, totalCount) = await _postRepository.GetPostsWithFiltersAsync(filter);
                var postResponses = posts.Select(MapPostToListResponse).ToList();

                var pagination = new PaginationResponse
                {
                    CurrentPage = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalRecords = totalCount,
                    TotalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize)
                };

                var response = new PostListWithPaginationResponse
                {
                    Items = postResponses,
                    Pagination = pagination
                };

                return ApiResponse<PostListWithPaginationResponse>.SuccessResponse(
                    response,
                    $"Retrieved {postResponses.Count} popular posts successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting popular posts");
                return ApiResponse<PostListWithPaginationResponse>.ErrorResponse(
                    "An error occurred while fetching popular posts", new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<PostResponse>> GetPublicPostByIdAsync(int postId, int? viewerUserId = null)
        {
            try
            {
                var post = await _postRepository.GetPostByIdAsync(postId);
                if (post == null || !post.IsActive || post.Status != "ACTIVE")
                {
                    return ApiResponse<PostResponse>.ErrorResponse("Post not found or not available");
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
                _logger.LogError(ex, "Error getting public post {PostId}", postId);
                return ApiResponse<PostResponse>.ErrorResponse("An error occurred while fetching the post",
                    new List<string> { ex.Message });
            }
        }

        // Private helper methods
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
                MainImageUrl = post.MainImageUrl
            };
        }

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
    }
}