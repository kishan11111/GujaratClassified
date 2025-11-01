namespace GujaratClassified.API.Models.Response
{
    public class LocalCardResponse
    {
        public int CardId { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? UserMobile { get; set; }

        // બિઝનેસ માહિતી
        public string BusinessName { get; set; }
        public string? BusinessNameGujarati { get; set; }
        public string? BusinessDescription { get; set; }
        public string? BusinessDescriptionGujarati { get; set; }

        // કેટેગરી
        public int CategoryId { get; set; }
        public string? CategoryNameGujarati { get; set; }
        public string? CategoryNameEnglish { get; set; }
        public int? SubCategoryId { get; set; }
        public string? SubCategoryNameGujarati { get; set; }
        public string? SubCategoryNameEnglish { get; set; }

        // કોન્ટેક્ટ માહિતી
        public string ContactPersonName { get; set; }
        public string PrimaryPhone { get; set; }
        public string? SecondaryPhone { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? Email { get; set; }

        // લોકેશન
        public int DistrictId { get; set; }
        public string? DistrictNameGujarati { get; set; }
        public string? DistrictNameEnglish { get; set; }
        public int TalukaId { get; set; }
        public string? TalukaNameGujarati { get; set; }
        public string? TalukaNameEnglish { get; set; }
        public int VillageId { get; set; }
        public string? VillageNameGujarati { get; set; }
        public string? VillageNameEnglish { get; set; }
        public string? FullAddress { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? DistanceKm { get; set; } // For nearby searches

        // કામના સમય
        public string? WorkingHours { get; set; }
        public string? WorkingDays { get; set; }
        public bool IsOpen24Hours { get; set; }

        // મીડિયા
        public string? ProfileImage { get; set; }
        public string? CoverImage { get; set; }
        public List<LocalCardImageResponse>? Images { get; set; }

        // સ્ટેટસ
        public bool IsActive { get; set; }
        public bool IsVerified { get; set; }

        // ટાઈમસ્ટેમ્પ
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class LocalCardImageResponse
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public int SortOrder { get; set; }
    }
}