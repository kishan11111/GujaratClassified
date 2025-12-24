namespace GujaratClassified.API.Models.Admin
{
    public class AdminLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class AdminDto
    {
        public int AdminId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Dashboard Statistics DTO
    public class DashboardStatsDto
    {
        // Today's statistics
        public int TodayTotalPosts { get; set; }
        public int TodayPendingPosts { get; set; }
        public int TodayApprovedPosts { get; set; }
        public int TodayRejectedPosts { get; set; }

        // Overall statistics
        public int TotalPosts { get; set; }
        public int TotalPendingPosts { get; set; }
        public int TotalApprovedPosts { get; set; }
        public int TotalRejectedPosts { get; set; }

        // User statistics
        public int TotalActiveUsers { get; set; }
        public int TodayNewUsers { get; set; }

        // Other statistics
        public int TotalFeaturedPosts { get; set; }
        public int ActiveCategories { get; set; }
    }

    // Post Admin View DTO
    public class PostAdminViewDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public string Condition { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // User Information
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }

        // Category Information
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }

        // Location Information
        public string DistrictName { get; set; }
        public string TalukaName { get; set; }
        public string VillageName { get; set; }
        public string Address { get; set; }

        // Images
        public string ImagePaths { get; set; }
        public string MainImagePath { get; set; }

        // Pagination
        public int TotalCount { get; set; }

        // Computed property for image array
        public List<string> GetImageList()
        {
            if (string.IsNullOrEmpty(ImagePaths))
                return new List<string>();

            return ImagePaths.Split(',').ToList();
        }
    }

    // Post List DTO for general listing
    public class PostListDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public string Condition { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public int IsActive { get; set; }
        public string StatusText { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // User Info
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }

        // Category
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }

        // Location
        public string FullLocation { get; set; }

        // Image
        public string MainImagePath { get; set; }
        public int TotalImages { get; set; }
    }

    // Post Detail DTO
    public class PostDetailDto
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public string Condition { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public int IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string ContactMethod { get; set; }
        public string ContactPhone { get; set; }

        // User Information
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public DateTime UserJoinDate { get; set; }
        public int UserTotalPosts { get; set; }

        // Category Information
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string SubCategoryName { get; set; }

        // Location Information
        public int? DistrictId { get; set; }
        public string DistrictName { get; set; }
        public int? TalukaId { get; set; }
        public string TalukaName { get; set; }
        public int? VillageId { get; set; }
        public string VillageName { get; set; }
        public string Address { get; set; }
    }

    // Post Image DTO
    public class PostImageDto
    {
        public int ImageId { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    // Admin Action History DTO
    public class AdminActionDto
    {
        public int ActionId { get; set; }
        public string Action { get; set; }
        public string Notes { get; set; }
        public DateTime ActionDate { get; set; }
        public string AdminName { get; set; }
    }

    // Action Request DTOs
    public class ApproveRejectDto
    {
        public string Notes { get; set; }
        public string RejectionReason { get; set; }
    }

    public class DeletePostDto
    {
        public string DeleteReason { get; set; }
    }

    public class BulkApproveDto
    {
        public List<int> PostIds { get; set; }
    }

    // Result DTOs
    public class ActionResultDto
    {
        public string Message { get; set; }
        public int Success { get; set; }
    }

    public class BulkActionResultDto : ActionResultDto
    {
        public int ApprovedCount { get; set; }
    }

    public class ToggleFeaturedResultDto : ActionResultDto
    {
        public int NewFeaturedStatus { get; set; }
    }

    // Category Statistics DTO
    public class CategoryStatsDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int TotalPosts { get; set; }
        public int PendingPosts { get; set; }
        public int ApprovedPosts { get; set; }
        public int RejectedPosts { get; set; }
        public int FeaturedPosts { get; set; }
        public decimal? AvgViews { get; set; }
    }

    // Filter DTOs
    public class PostFilterDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int? Status { get; set; }
        public int? CategoryId { get; set; }
        public string SearchText { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }

    // Pagination Response
    public class PaginatedResponse<T>
    {
        public bool Success { get; set; }
        public List<T> Data { get; set; }
        public PaginationInfo Pagination { get; set; }
    }

    public class PaginationInfo
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }

    // API Response Wrapper
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public object Errors { get; set; }
    }

    // Post Status Enum
    public enum PostStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Deleted = 3
    }

    // Admin Role Enum
    public enum AdminRole
    {
        Admin,
        SuperAdmin,
        Moderator
    }
}
