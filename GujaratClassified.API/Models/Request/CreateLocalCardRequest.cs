// Models/Request/CreateLocalCardRequest.cs
using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class CreateLocalCardRequest
    {
        [Required(ErrorMessage = "Business name is required")]
        [StringLength(200, ErrorMessage = "Business name cannot exceed 200 characters")]
        public string BusinessName { get; set; }

        [StringLength(200, ErrorMessage = "Business name in Gujarati cannot exceed 200 characters")]
        public string? BusinessNameGujarati { get; set; }

        [StringLength(1000, ErrorMessage = "Business description cannot exceed 1000 characters")]
        public string? BusinessDescription { get; set; }

        [StringLength(1000, ErrorMessage = "Business description in Gujarati cannot exceed 1000 characters")]
        public string? BusinessDescriptionGujarati { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        public int? SubCategoryId { get; set; }

        [Required(ErrorMessage = "Contact person name is required")]
        [StringLength(100, ErrorMessage = "Contact person name cannot exceed 100 characters")]
        public string ContactPersonName { get; set; }

        [Required(ErrorMessage = "Primary phone is required")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Primary phone must be between 10-15 digits")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid mobile number")]
        public string PrimaryPhone { get; set; }

        [StringLength(15, MinimumLength = 10, ErrorMessage = "Secondary phone must be between 10-15 digits")]
        public string? SecondaryPhone { get; set; }

        [StringLength(15, MinimumLength = 10, ErrorMessage = "WhatsApp number must be between 10-15 digits")]
        public string? WhatsAppNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "District is required")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Taluka is required")]
        public int TalukaId { get; set; }

        [Required(ErrorMessage = "Village is required")]
        public int VillageId { get; set; }

        [StringLength(500, ErrorMessage = "Full address cannot exceed 500 characters")]
        public string? FullAddress { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [StringLength(200, ErrorMessage = "Working hours cannot exceed 200 characters")]
        public string? WorkingHours { get; set; }

        [StringLength(100, ErrorMessage = "Working days cannot exceed 100 characters")]
        public string? WorkingDays { get; set; }

        public bool IsOpen24Hours { get; set; } = false;

        public string? ProfileImage { get; set; }
        public string? CoverImage { get; set; }
        public List<string>? AdditionalImages { get; set; }
    }
}
