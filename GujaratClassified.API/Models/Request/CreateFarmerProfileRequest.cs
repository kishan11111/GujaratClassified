using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class CreateFarmerProfileRequest
    {
        [StringLength(200, ErrorMessage = "Farm name cannot exceed 200 characters")]
        public string? FarmName { get; set; }

        [Range(0.1, 50000, ErrorMessage = "Total farm area must be between 0.1 and 50000 acres")]
        public decimal? TotalFarmArea { get; set; }

        public List<string>? MainCrops { get; set; }

        [StringLength(50, ErrorMessage = "Farming experience cannot exceed 50 characters")]
        public string? FarmingExperience { get; set; } // Beginner, Intermediate, Expert

        public List<string>? SpecialtyAreas { get; set; }

        [StringLength(1000, ErrorMessage = "Bio cannot exceed 1000 characters")]
        public string? Bio { get; set; }

        public List<string>? Achievements { get; set; }
    }
}