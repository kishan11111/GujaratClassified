namespace GujaratClassified.API.Models.Entity
{
    public class SubCategory
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string SubCategoryNameGujarati { get; set; }
        public string SubCategoryNameEnglish { get; set; }
        public string? SubCategoryIcon { get; set; }
        public string? SubCategoryImage { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property (not stored in DB, used for joins)
        public string? CategoryName { get; set; }
    }
}
