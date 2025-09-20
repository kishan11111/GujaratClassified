namespace GujaratClassified.API.Models.Entity
{
    public class PostComment
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int? ParentCommentId { get; set; } // For replies
        public string CommentText { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsBlocked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (not stored in DB, used for joins)
        public string? UserName { get; set; }
        public string? UserMobile { get; set; }
        public bool? UserVerified { get; set; }
        public int ReplyCount { get; set; } = 0;
        public List<PostComment>? Replies { get; set; }
    }
}