using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class UpdateAdvertisementRequest
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

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

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Range(0, 10, ErrorMessage = "Priority must be between 0 and 10")]
        public int Priority { get; set; }

        // Targeting options
        public int? TargetDistrictId { get; set; }
        public int? TargetTalukaId { get; set; }
        public int? TargetCategoryId { get; set; }
        public string? TargetAgeGroup { get; set; }
        public string? TargetGender { get; set; }
    }
}
