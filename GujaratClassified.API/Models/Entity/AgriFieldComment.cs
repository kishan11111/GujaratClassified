namespace GujaratClassified.API.Models.Entity
{
    public class AgriFieldComment
    {
        public int CommentId { get; set; }
        public int AgriFieldId { get; set; }
        public int UserId { get; set; }
        public int? ParentCommentId { get; set; } // For nested replies
        public string CommentText { get; set; }
        public string? CommentType { get; set; } // Question, Advice, Appreciation, Help
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public string? UserName { get; set; }
        public string? UserProfileImage { get; set; }
        public bool? UserVerified { get; set; }
        public List<AgriFieldComment>? Replies { get; set; }
    }
}
