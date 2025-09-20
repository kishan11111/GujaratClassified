using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class AdInquiryRequest
    {
        [Required(ErrorMessage = "Advertisement ID is required")]
        public int AdId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        public string InquirerName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string InquirerPhone { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? InquirerEmail { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; }
    }
}
