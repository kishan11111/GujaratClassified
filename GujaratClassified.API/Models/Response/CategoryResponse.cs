namespace GujaratClassified.API.Models.Response
{
    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public string CategoryNameGujarati { get; set; }
        public string CategoryNameEnglish { get; set; }
        public string? CategoryIcon { get; set; }
        public string? CategoryImage { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public int SubCategoryCount { get; set; }
    }
}
