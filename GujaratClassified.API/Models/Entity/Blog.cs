namespace GujaratClassified.API.Models.Entity
{
    public class Blog
    {
        public int BlogId { get; set; }
        public string TitleGujarati { get; set; }
        public string TitleEnglish { get; set; }
        public string ContentGujarati { get; set; }
        public string ContentEnglish { get; set; }
        public string? ThumbnailImage { get; set; }
        public string? Author { get; set; }
        public int ViewCount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }

        // Navigation properties (not stored in DB, used for joins)
        public string? CategoryName { get; set; }
        public List<string>? Tags { get; set; }
    }
}