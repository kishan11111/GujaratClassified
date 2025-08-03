using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class PostStatusRequest
    {
        [Required(ErrorMessage = "Status is required")]
        [RegularExpression(@"^(ACTIVE|SOLD|EXPIRED|BLOCKED)$", ErrorMessage = "Invalid status")]
        public string Status { get; set; }
    }
}
