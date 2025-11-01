using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Services.Implementations
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ILogger<BlogService> _logger;

        public BlogService(IBlogRepository blogRepository, ILogger<BlogService> logger)
        {
            _blogRepository = blogRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<BlogListWithPaginationResponse>> GetAllBlogsAsync(BlogFilterRequest filter)
        {
            try
            {
                var (blogs, totalCount) = await _blogRepository.GetAllBlogsAsync(filter);

                var blogList = blogs.Select(b => new BlogListResponse
                {
                    BlogId = b.BlogId,
                    TitleGujarati = b.TitleGujarati,
                    TitleEnglish = b.TitleEnglish,
                    ThumbnailImage = b.ThumbnailImage,
                    Author = b.Author,
                    ViewCount = b.ViewCount,
                    IsFeatured = b.IsFeatured,
                    CreatedAt = b.CreatedAt
                }).ToList();

                var totalPages = (int)Math.Ceiling((double)totalCount / filter.PageSize);

                var response = new BlogListWithPaginationResponse
                {
                    Blogs = blogList,
                    TotalCount = totalCount,
                    PageNumber = filter.PageNumber,
                    PageSize = filter.PageSize,
                    TotalPages = totalPages
                };

                return ApiResponse<BlogListWithPaginationResponse>.SuccessResponse(
                    response,
                    "Blogs retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blogs");
                return ApiResponse<BlogListWithPaginationResponse>.ErrorResponse(
                    "An error occurred while retrieving blogs",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<BlogDetailResponse>> GetBlogByIdAsync(int blogId)
        {
            try
            {
                var blog = await _blogRepository.GetBlogByIdAsync(blogId);

                if (blog == null)
                {
                    return ApiResponse<BlogDetailResponse>.ErrorResponse("Blog not found");
                }

                // Increment view count asynchronously
                _ = _blogRepository.IncrementViewCountAsync(blogId);

                // Get related blogs
                var relatedBlogs = await _blogRepository.GetRelatedBlogsAsync(blogId, 4);

                var response = new BlogDetailResponse
                {
                    BlogId = blog.BlogId,
                    TitleGujarati = blog.TitleGujarati,
                    TitleEnglish = blog.TitleEnglish,
                    ContentGujarati = blog.ContentGujarati,
                    ContentEnglish = blog.ContentEnglish,
                    ThumbnailImage = blog.ThumbnailImage,
                    Author = blog.Author,
                    ViewCount = blog.ViewCount,
                    IsFeatured = blog.IsFeatured,
                    CreatedAt = blog.CreatedAt,
                    UpdatedAt = blog.UpdatedAt,
                    RelatedBlogs = relatedBlogs.Select(rb => new BlogListResponse
                    {
                        BlogId = rb.BlogId,
                        TitleGujarati = rb.TitleGujarati,
                        TitleEnglish = rb.TitleEnglish,
                        ThumbnailImage = rb.ThumbnailImage,
                        Author = rb.Author,
                        ViewCount = rb.ViewCount,
                        IsFeatured = rb.IsFeatured,
                        CreatedAt = rb.CreatedAt
                    }).ToList()
                };

                return ApiResponse<BlogDetailResponse>.SuccessResponse(response, "Blog retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blog with ID {BlogId}", blogId);
                return ApiResponse<BlogDetailResponse>.ErrorResponse(
                    "An error occurred while retrieving the blog",
                    new List<string> { ex.Message }
                );
            }
        }

        public async Task<ApiResponse<List<BlogListResponse>>> GetFeaturedBlogsAsync(int count = 5)
        {
            try
            {
                var blogs = await _blogRepository.GetFeaturedBlogsAsync(count);

                var blogList = blogs.Select(b => new BlogListResponse
                {
                    BlogId = b.BlogId,
                    TitleGujarati = b.TitleGujarati,
                    TitleEnglish = b.TitleEnglish,
                    ThumbnailImage = b.ThumbnailImage,
                    Author = b.Author,
                    ViewCount = b.ViewCount,
                    IsFeatured = b.IsFeatured,
                    CreatedAt = b.CreatedAt
                }).ToList();

                return ApiResponse<List<BlogListResponse>>.SuccessResponse(
                    blogList,
                    "Featured blogs retrieved successfully"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving featured blogs");
                return ApiResponse<List<BlogListResponse>>.ErrorResponse(
                    "An error occurred while retrieving featured blogs",
                    new List<string> { ex.Message }
                );
            }
        }
    }
}