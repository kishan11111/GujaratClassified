namespace GujaratClassified.API.Models.Entity
{
    public class AgriField
    {
        public int AgriFieldId { get; set; }
        public int UserId { get; set; }
        public string FarmName { get; set; }
        public int DistrictId { get; set; }
        public int TalukaId { get; set; }
        public int VillageId { get; set; }
        public string? Address { get; set; }
        public string CropType { get; set; } // Cotton, Wheat, Sugarcane, Rice, etc.
        public string FarmingMethod { get; set; } // Organic, Traditional, Modern, Hydroponic
        public string Season { get; set; } // Kharif, Rabi, Summer, Year-round
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? FarmSizeAcres { get; set; }
        public string? SoilType { get; set; } // Black, Red, Sandy, Clay, Loamy
        public string? WaterSource { get; set; } // Bore, Canal, Rain-fed, Drip
        public DateTime? PlantingDate { get; set; }
        public DateTime? ExpectedHarvestDate { get; set; }
        public string? Tags { get; set; } // JSON array: ["OrganicFarming", "SugarcaneFarming"]
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; } = false;
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, ARCHIVED, BLOCKED
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int ViewCount { get; set; } = 0;
        public int LikeCount { get; set; } = 0;
        public int CommentCount { get; set; } = 0;
        public int FollowerCount { get; set; } = 0;

        // Navigation properties (not stored in DB, used for joins)
        public string? FarmerName { get; set; }
        public string? FarmerMobile { get; set; }
        public string? FarmerProfileImage { get; set; }
        public bool? FarmerVerified { get; set; }
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
        public string? VillageName { get; set; }
        public List<AgriFieldImage>? Images { get; set; }
        public List<AgriFieldVideo>? Videos { get; set; }
        public List<AgriFieldComment>? Comments { get; set; }
        public bool? IsLiked { get; set; } // For current user
        public bool? IsFollowed { get; set; } // For current user
    }
}
