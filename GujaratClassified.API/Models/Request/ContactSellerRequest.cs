using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class ContactSellerRequest
    {
        [Required(ErrorMessage = "Post ID is required")]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [StringLength(1000, ErrorMessage = "Message cannot exceed 1000 characters")]
        public string Message { get; set; }

        [StringLength(100, ErrorMessage = "Buyer name cannot exceed 100 characters")]
        public string? BuyerName { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(15, ErrorMessage = "Phone number cannot exceed 15 characters")]
        public string? BuyerPhone { get; set; }
    }
}
