using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class PostFilterRequest
    {
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? DistrictId { get; set; }
        public int? TalukaId { get; set; }
        public int? VillageId { get; set; }

        [Range(0, 999999999, ErrorMessage = "Invalid minimum price")]
        public decimal? MinPrice { get; set; }

        [Range(0, 999999999, ErrorMessage = "Invalid maximum price")]
        public decimal? MaxPrice { get; set; }

        public string? Condition { get; set; }
        public string? PriceType { get; set; }
        public bool? IsFeatured { get; set; }
        public string? Status { get; set; } = "ACTIVE";

        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string? SearchTerm { get; set; }

        [RegularExpression(@"^(NEWEST|OLDEST|PRICE_LOW|PRICE_HIGH|POPULAR)$", ErrorMessage = "Invalid sort option")]
        public string SortBy { get; set; } = "NEWEST";

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 20;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;
    }
}
