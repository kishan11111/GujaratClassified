namespace GujaratClassified.API.Models.Entity
{
    public class AdClick
    {
        public int ClickId { get; set; }
        public int AdId { get; set; }
        public int? UserId { get; set; } // Null for guest users
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public DateTime ClickedAt { get; set; } = DateTime.UtcNow;
    }
}
