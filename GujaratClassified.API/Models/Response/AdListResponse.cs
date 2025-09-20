namespace GujaratClassified.API.Models.Response
{
    public class AdListResponse
    {
        public int AdId { get; set; }
        public string Title { get; set; }
        public string AdType { get; set; }
        public string Position { get; set; }
        public string? ImageUrl { get; set; }
        public string? CompanyName { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Computed properties
        public int DaysRemaining => Math.Max(0, (EndDate - DateTime.UtcNow).Days);
        public bool IsExpired => DateTime.UtcNow > EndDate;
        public string StatusBadge => IsExpired ? "EXPIRED" : Status;
    }
}
