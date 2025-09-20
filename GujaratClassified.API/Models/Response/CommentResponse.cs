namespace GujaratClassified.API.Models.Response
{
    public class CommentResponse
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public string CommentText { get; set; }
        public string UserName { get; set; }
        public bool UserVerified { get; set; }
        public int ReplyCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool CanEdit { get; set; } // If current user can edit this comment
        public bool CanDelete { get; set; } // If current user can delete this comment
        public List<CommentResponse>? Replies { get; set; }
    }
}