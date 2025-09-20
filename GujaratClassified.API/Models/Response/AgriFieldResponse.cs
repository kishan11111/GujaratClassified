namespace GujaratClassified.API.Models.Response
{
    public class AgriFieldResponse
    {
        public int AgriFieldId { get; set; }
        public int UserId { get; set; }
        public string FarmName { get; set; }
        public int DistrictId { get; set; }
        public int TalukaId { get; set; }
        public int VillageId { get; set; }
        public string? Address { get; set; }
        public string CropType { get; set; }
        public string FarmingMethod { get; set; }
        public string Season { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? FarmSizeAcres { get; set; }
        public string? SoilType { get; set; }
        public string? WaterSource { get; set; }
        public DateTime? PlantingDate { get; set; }
        public DateTime? ExpectedHarvestDate { get; set; }
        public List<string>? Tags { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public int FollowerCount { get; set; }

        // Farmer details
        public string? FarmerName { get; set; }
        public string? FarmerMobile { get; set; }
        public string? FarmerProfileImage { get; set; }
        public bool? FarmerVerified { get; set; }

        // Location details
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
        public string? VillageName { get; set; }

        // Media
        public List<AgriFieldImageResponse>? Images { get; set; }
        public List<AgriFieldVideoResponse>? Videos { get; set; }

        // User interactions
        public bool? IsLiked { get; set; }
        public bool? IsFollowed { get; set; }

        // Comments (for detailed view)
        public List<AgriFieldCommentResponse>? Comments { get; set; }
    }

    public class AgriFieldImageResponse
    {
        public int AgriImageId { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public string? ImageType { get; set; }
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AgriFieldVideoResponse
    {
        public int AgriVideoId { get; set; }
        public string VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Caption { get; set; }
        public string? VideoType { get; set; }
        public int SortOrder { get; set; }
        public int? DurationSeconds { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AgriFieldCommentResponse
    {
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public string CommentText { get; set; }
        public string? CommentType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // User details
        public string? UserName { get; set; }
        public string? UserProfileImage { get; set; }
        public bool? UserVerified { get; set; }

        // Nested replies
        public List<AgriFieldCommentResponse>? Replies { get; set; }
    }
}