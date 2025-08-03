using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class CreatePostRequest
    {
        [Required(ErrorMessage = "Category is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Sub-category is required")]
        public int SubCategoryId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 999999999, ErrorMessage = "Price must be between 0 and 999,999,999")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Price type is required")]
        [RegularExpression(@"^(FIXED|NEGOTIABLE|ON_CALL)$", ErrorMessage = "Price type must be FIXED, NEGOTIABLE, or ON_CALL")]
        public string PriceType { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        [RegularExpression(@"^(NEW|LIKE_NEW|GOOD|FAIR|POOR)$", ErrorMessage = "Invalid condition")]
        public string Condition { get; set; }

        [Required(ErrorMessage = "District is required")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Taluka is required")]
        public int TalukaId { get; set; }

        [Required(ErrorMessage = "Village is required")]
        public int VillageId { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Contact method is required")]
        [RegularExpression(@"^(PHONE|CHAT|BOTH)$", ErrorMessage = "Contact method must be PHONE, CHAT, or BOTH")]
        public string ContactMethod { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string? ContactPhone { get; set; }

        public bool IsFeatured { get; set; } = false; // Only for premium users
    }
}
