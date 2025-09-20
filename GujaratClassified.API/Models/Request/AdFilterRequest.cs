using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class AdFilterRequest
    {
        public string? AdType { get; set; }
        public string? Position { get; set; }
        public string? Status { get; set; }
        public int? TargetDistrictId { get; set; }
        public int? TargetCategoryId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPaid { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
        public string? SearchTerm { get; set; }

        [RegularExpression(@"^(NEWEST|OLDEST|PRICE_HIGH|PRICE_LOW|PRIORITY|POPULAR)$", ErrorMessage = "Invalid sort option")]
        public string SortBy { get; set; } = "PRIORITY";

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 20;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;
    }
}
