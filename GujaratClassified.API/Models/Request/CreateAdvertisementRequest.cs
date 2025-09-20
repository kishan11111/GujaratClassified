using System.ComponentModel.DataAnnotations;
namespace GujaratClassified.API.Models.Request
{
    public class CreateAdvertisementRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Ad type is required")]
        [RegularExpression(@"^(BANNER|SIDEBAR|POPUP|INLINE|FEATURED)$", ErrorMessage = "Invalid ad type")]
        public string AdType { get; set; }

        [Required(ErrorMessage = "Position is required")]
        [RegularExpression(@"^(HEADER|FOOTER|HOME_TOP|HOME_MIDDLE|CATEGORY_TOP|POST_DETAIL|SEARCH_RESULTS)$", ErrorMessage = "Invalid position")]
        public string Position { get; set; }

        [Url(ErrorMessage = "Invalid target URL format")]
        public string? TargetUrl { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string? ContactPhone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? ContactEmail { get; set; }

        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string? CompanyName { get; set; }

        [StringLength(500, ErrorMessage = "Company address cannot exceed 500 characters")]
        public string? CompanyAddress { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 999999, ErrorMessage = "Price must be between 0 and 999,999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Price type is required")]
        [RegularExpression(@"^(PER_DAY|PER_WEEK|PER_MONTH|PER_CLICK|PER_IMPRESSION|ONE_TIME)$", ErrorMessage = "Invalid price type")]
        public string PriceType { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Range(0, 10, ErrorMessage = "Priority must be between 0 and 10")]
        public int Priority { get; set; } = 0;

        // Targeting options
        public int? TargetDistrictId { get; set; }
        public int? TargetTalukaId { get; set; }
        public int? TargetCategoryId { get; set; }

        [RegularExpression(@"^(18-25|26-35|36-45|46\+)$", ErrorMessage = "Invalid age group")]
        public string? TargetAgeGroup { get; set; }

        [RegularExpression(@"^(M|F|ALL)$", ErrorMessage = "Invalid gender")]
        public string? TargetGender { get; set; }
    }
}
