namespace GujaratClassified.API.Models.Entity
{
    public class Advertisement
    {
        public int AdId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string AdType { get; set; } // BANNER, SIDEBAR, POPUP, INLINE, FEATURED
        public string Position { get; set; } // HEADER, FOOTER, HOME_TOP, HOME_MIDDLE, CATEGORY_TOP, POST_DETAIL, SEARCH_RESULTS
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? TargetUrl { get; set; } // Where user goes when clicking ad
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyAddress { get; set; }
        public decimal Price { get; set; } // Cost of advertisement
        public string PriceType { get; set; } // PER_DAY, PER_WEEK, PER_MONTH, PER_CLICK, PER_IMPRESSION, ONE_TIME
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsPaid { get; set; } = false;
        public string Status { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED, EXPIRED, PAUSED
        public int Priority { get; set; } = 0; // Higher number = higher priority
        public int ViewCount { get; set; } = 0;
        public int ClickCount { get; set; } = 0;
        public int InquiryCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Targeting options
        public int? TargetDistrictId { get; set; }
        public int? TargetTalukaId { get; set; }
        public int? TargetCategoryId { get; set; }
        public string? TargetAgeGroup { get; set; } // 18-25, 26-35, 36-45, 46+
        public string? TargetGender { get; set; } // M, F, ALL

        // Navigation properties (not stored in DB, used for joins)
        public string? TargetDistrictName { get; set; }
        public string? TargetTalukaName { get; set; }
        public string? TargetCategoryName { get; set; }
    }
}
