using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class AdStatusRequest
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression(@"^(PENDING|APPROVED|REJECTED|EXPIRED|PAUSED)$", ErrorMessage = "Invalid status")]
        public string Status { get; set; }

        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        public string? Reason { get; set; }
    }
}
