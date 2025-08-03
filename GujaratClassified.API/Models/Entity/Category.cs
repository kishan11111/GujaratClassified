namespace GujaratClassified.API.Models.Entity
{
    public class Category
    {
        public int CategoryId { get; set; }
        public string CategoryNameGujarati { get; set; }
        public string CategoryNameEnglish { get; set; }
        public string? CategoryIcon { get; set; }
        public string? CategoryImage { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // For API response
        public int SubCategoryCount { get; set; } = 0;
    }
}
