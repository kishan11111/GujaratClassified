using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class UpdateCommentRequest
    {
        [Required]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 1000 characters")]
        public string CommentText { get; set; }
    }
}