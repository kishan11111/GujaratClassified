namespace GujaratClassified.API.Models.Response
{
    public class AdInquiryResponse
    {
        public int InquiryId { get; set; }
        public int AdId { get; set; }
        public int UserId { get; set; }
        public string InquirerName { get; set; }
        public string InquirerPhone { get; set; }
        public string? InquirerEmail { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? RespondedAt { get; set; }

        // Navigation properties
        public string? AdTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? UserName { get; set; }

        // Computed properties
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - CreatedAt;
                if (timeSpan.Days > 0) return $"{timeSpan.Days}d ago";
                if (timeSpan.Hours > 0) return $"{timeSpan.Hours}h ago";
                if (timeSpan.Minutes > 0) return $"{timeSpan.Minutes}m ago";
                return "Just now";
            }
        }
    }
}
