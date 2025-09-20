namespace GujaratClassified.API.Models.Response
{
    public class AdAnalyticsResponse
    {
        public int AdId { get; set; }
        public string Title { get; set; }
        public int TotalViews { get; set; }
        public int TotalClicks { get; set; }
        public int TotalInquiries { get; set; }
        public decimal ClickThroughRate { get; set; }
        public decimal InquiryRate { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<DailyAdStats> DailyStats { get; set; } = new List<DailyAdStats>();
    }

    public class DailyAdStats
    {
        public DateTime Date { get; set; }
        public int Views { get; set; }
        public int Clicks { get; set; }
        public int Inquiries { get; set; }
    }
}
