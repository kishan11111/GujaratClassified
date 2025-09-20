namespace GujaratClassified.API.Models.Entity
{
    public class AdView
    {
        public int ViewId { get; set; }
        public int AdId { get; set; }
        public int? UserId { get; set; } // Null for guest users
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string Position { get; set; } // Where the ad was viewed
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
