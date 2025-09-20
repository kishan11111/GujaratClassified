using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class CreateAgriFieldCommentRequest
    {
        [Required(ErrorMessage = "Comment text is required")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters")]
        public string CommentText { get; set; }

        public int? ParentCommentId { get; set; }

        [StringLength(50, ErrorMessage = "Comment type cannot exceed 50 characters")]
        public string? CommentType { get; set; } // Question, Advice, Appreciation, Help
    }
}