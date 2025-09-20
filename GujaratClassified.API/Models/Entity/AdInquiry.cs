namespace GujaratClassified.API.Models.Entity
{
    public class AdInquiry
    {
        public int InquiryId { get; set; }
        public int AdId { get; set; }
        public int UserId { get; set; }
        public string InquirerName { get; set; }
        public string InquirerPhone { get; set; }
        public string? InquirerEmail { get; set; }
        public string Message { get; set; }
        public string Status { get; set; } = "NEW"; // NEW, READ, RESPONDED, CLOSED
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? RespondedAt { get; set; }

        // Navigation properties
        public string? AdTitle { get; set; }
        public string? CompanyName { get; set; }
        public string? UserName { get; set; }
    }
}
