using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class GetCommentsRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Post ID must be greater than 0")]
        public int PostId { get; set; }

        [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
        public int PageSize { get; set; } = 10;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        public string? SortBy { get; set; } = "NEWEST"; // NEWEST, OLDEST
        public bool IncludeReplies { get; set; } = true;
    }
}