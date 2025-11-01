namespace GujaratClassified.API.Models.Response
{
    public class LocalCardSubCategoryResponse
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string SubCategoryNameGujarati { get; set; }
        public string SubCategoryNameEnglish { get; set; }
        public string? SubCategoryIcon { get; set; }
        public string? Description { get; set; }
        public int TotalCards { get; set; }
    }
}