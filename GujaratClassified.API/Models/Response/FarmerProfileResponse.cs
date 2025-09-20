namespace GujaratClassified.API.Models.Response
{
    public class FarmerProfileResponse
    {
        public int FarmerProfileId { get; set; }
        public int UserId { get; set; }
        public string? FarmName { get; set; }
        public decimal? TotalFarmArea { get; set; }
        public List<string>? MainCrops { get; set; }
        public string? FarmingExperience { get; set; }
        public List<string>? SpecialtyAreas { get; set; }
        public string? Bio { get; set; }
        public List<string>? Achievements { get; set; }
        public bool IsVerifiedFarmer { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Statistics
        public int TotalPosts { get; set; }
        public int TotalFollowers { get; set; }
        public int TotalLikes { get; set; }
        public int HelpfulAnswers { get; set; }

        // User details
        public string? UserName { get; set; }
        public string? UserProfileImage { get; set; }
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }

        // Recent posts
        public List<AgriFieldResponse>? RecentPosts { get; set; }
    }
}