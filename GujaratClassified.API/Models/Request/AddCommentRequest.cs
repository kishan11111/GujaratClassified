using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class AddCommentRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Post ID must be greater than 0")]
        public int PostId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 1000 characters")]
        public string CommentText { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Parent Comment ID must be greater than 0")]
        public int? ParentCommentId { get; set; } // For replies
    }
}