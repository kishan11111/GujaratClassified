using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class SendEmailRequest
    {
        [Required(ErrorMessage = "Purpose is required")]
        public string Purpose { get; set; } = string.Empty; // SUPPORT, FEEDBACK, CONTACT_SELLER, INQUIRY

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = string.Empty;

        public string? Phone { get; set; }

        [Required(ErrorMessage = "Subject is required")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [MinLength(10, ErrorMessage = "Message must be at least 10 characters")]
        public string Message { get; set; } = string.Empty;

        // Optional fields for specific purposes
        public int? PostId { get; set; }
        public string? PostTitle { get; set; }
        public int? Rating { get; set; } // For feedback (1-5)
        public string? PageUrl { get; set; }
    }
}
