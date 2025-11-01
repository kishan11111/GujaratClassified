namespace GujaratClassified.API.Models.Response
{
    public class BlogResponse
    {
        public int BlogId { get; set; }
        public string TitleGujarati { get; set; }
        public string TitleEnglish { get; set; }
        public string ContentGujarati { get; set; }
        public string ContentEnglish { get; set; }
        public string? ThumbnailImage { get; set; }
        public string? Author { get; set; }
        public int ViewCount { get; set; }
        public bool IsActive { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class BlogListResponse
    {
        public int BlogId { get; set; }
        public string TitleGujarati { get; set; }
        public string TitleEnglish { get; set; }
        public string? ThumbnailImage { get; set; }
        public string? Author { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class BlogListWithPaginationResponse
    {
        public List<BlogListResponse> Blogs { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class BlogDetailResponse
    {
        public int BlogId { get; set; }
        public string TitleGujarati { get; set; }
        public string TitleEnglish { get; set; }
        public string ContentGujarati { get; set; }
        public string ContentEnglish { get; set; }
        public string? ThumbnailImage { get; set; }
        public string? Author { get; set; }
        public int ViewCount { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<BlogListResponse>? RelatedBlogs { get; set; }
    }
}