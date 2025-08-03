namespace GujaratClassified.API.Models.Response
{
    public class SubCategoryResponse
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string SubCategoryNameGujarati { get; set; }
        public string SubCategoryNameEnglish { get; set; }
        public string? SubCategoryIcon { get; set; }
        public string? SubCategoryImage { get; set; }
        public string? Description { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public string? CategoryName { get; set; }
    }
}
