using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class BlogFilterRequest
    {
        public string? SearchKeyword { get; set; }
        public bool? IsActive { get; set; } = true;
        public bool? IsFeatured { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "CreatedAt";
        public string SortOrder { get; set; } = "DESC";
    }
}