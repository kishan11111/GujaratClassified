using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class LikeToggleRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Post ID must be greater than 0")]
        public int PostId { get; set; }
    }
}