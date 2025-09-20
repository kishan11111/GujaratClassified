namespace GujaratClassified.API.Models.Response
{
    public class AdvertisementResponse
    {
        public int AdId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string AdType { get; set; }
        public string Position { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? TargetUrl { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPaid { get; set; }
        public string Status { get; set; }
        public int Priority { get; set; }
        public int ViewCount { get; set; }
        public int ClickCount { get; set; }
        public int InquiryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Targeting info
        public int? TargetDistrictId { get; set; }
        public int? TargetTalukaId { get; set; }
        public int? TargetCategoryId { get; set; }
        public string? TargetAgeGroup { get; set; }
        public string? TargetGender { get; set; }
        public string? TargetDistrictName { get; set; }
        public string? TargetTalukaName { get; set; }
        public string? TargetCategoryName { get; set; }

        // Computed properties
        public int DaysRemaining => Math.Max(0, (EndDate - DateTime.UtcNow).Days);
        public bool IsExpired => DateTime.UtcNow > EndDate;
        public bool IsRunning => DateTime.UtcNow >= StartDate && DateTime.UtcNow <= EndDate && IsActive && Status == "APPROVED";
        public decimal ClickThroughRate => ViewCount > 0 ? (decimal)ClickCount / ViewCount * 100 : 0;
    }
}
