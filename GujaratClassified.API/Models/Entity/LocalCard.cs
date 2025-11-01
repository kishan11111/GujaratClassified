namespace GujaratClassified.API.Models.Entity
{
    public class LocalCard
    {
        public int CardId { get; set; }
        public int UserId { get; set; }

        // બિઝનેસ માહિતી
        public string BusinessName { get; set; }
        public string? BusinessNameGujarati { get; set; }
        public string? BusinessDescription { get; set; }
        public string? BusinessDescriptionGujarati { get; set; }

        // કેટેગરી
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }

        // કોન્ટેક્ટ માહિતી
        public string ContactPersonName { get; set; }
        public string PrimaryPhone { get; set; }
        public string? SecondaryPhone { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Email { get; set; }

        // લોકેશન
        public int DistrictId { get; set; }
        public int TalukaId { get; set; }
        public int VillageId { get; set; }
        public string? FullAddress { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // કામના સમય
        public string? WorkingHours { get; set; }
        public string? WorkingDays { get; set; }
        public bool IsOpen24Hours { get; set; } = false;

        // મીડિયા
        public string? ProfileImage { get; set; }
        public string? CoverImage { get; set; }

        // સ્ટેટસ
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;

        // ટાઈમસ્ટેમ્પ
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (not stored in DB, used for joins and display)
        public string? UserName { get; set; }
        public string? UserMobile { get; set; }
        public string? CategoryNameGujarati { get; set; }
        public string? CategoryNameEnglish { get; set; }
        public string? SubCategoryNameGujarati { get; set; }
        public string? SubCategoryNameEnglish { get; set; }
        public string? DistrictNameGujarati { get; set; }
        public string? DistrictNameEnglish { get; set; }
        public string? TalukaNameGujarati { get; set; }
        public string? TalukaNameEnglish { get; set; }
        public string? VillageNameGujarati { get; set; }
        public string? VillageNameEnglish { get; set; }
        public decimal? DistanceKm { get; set; } // Calculated field for nearby search
        public List<LocalCardImage>? Images { get; set; }
    }
}