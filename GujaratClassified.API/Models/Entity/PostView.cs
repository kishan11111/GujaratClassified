namespace GujaratClassified.API.Models.Entity
{
    public class PostView
    {
        public int ViewId { get; set; }
        public int PostId { get; set; }
        public int? UserId { get; set; } // Null for guest users
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
