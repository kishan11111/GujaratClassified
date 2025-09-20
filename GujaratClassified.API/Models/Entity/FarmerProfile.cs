namespace GujaratClassified.API.Models.Entity
{
    public class FarmerProfile
    {
        public int FarmerProfileId { get; set; }
        public int UserId { get; set; }
        public string? FarmName { get; set; }
        public decimal? TotalFarmArea { get; set; }
        public string? MainCrops { get; set; } // JSON array
        public string? FarmingExperience { get; set; } // Beginner, Intermediate, Expert
        public string? SpecialtyAreas { get; set; } // JSON array: ["Organic", "Pest Control"]
        public string? Bio { get; set; }
        public string? Achievements { get; set; } // JSON array
        public bool IsVerifiedFarmer { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Statistics
        public int TotalPosts { get; set; } = 0;
        public int TotalFollowers { get; set; } = 0;
        public int TotalLikes { get; set; } = 0;
        public int HelpfulAnswers { get; set; } = 0;

        // Navigation properties
        public string? UserName { get; set; }
        public string? UserProfileImage { get; set; }
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
    }
}
