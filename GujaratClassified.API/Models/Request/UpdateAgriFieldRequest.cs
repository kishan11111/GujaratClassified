using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class UpdateAgriFieldRequest
    {
        [StringLength(200, ErrorMessage = "Farm name cannot exceed 200 characters")]
        public string? FarmName { get; set; }

        [StringLength(300, ErrorMessage = "Title cannot exceed 300 characters")]
        public string? Title { get; set; }

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string? Description { get; set; }

        public int? DistrictId { get; set; }
        public int? TalukaId { get; set; }
        public int? VillageId { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "Crop type cannot exceed 100 characters")]
        public string? CropType { get; set; }

        [StringLength(50, ErrorMessage = "Farming method cannot exceed 50 characters")]
        public string? FarmingMethod { get; set; }

        [StringLength(50, ErrorMessage = "Season cannot exceed 50 characters")]
        public string? Season { get; set; }

        [Range(0.1, 10000, ErrorMessage = "Farm size must be between 0.1 and 10000 acres")]
        public decimal? FarmSizeAcres { get; set; }

        [StringLength(50, ErrorMessage = "Soil type cannot exceed 50 characters")]
        public string? SoilType { get; set; }

        [StringLength(100, ErrorMessage = "Water source cannot exceed 100 characters")]
        public string? WaterSource { get; set; }

        public DateTime? PlantingDate { get; set; }
        public DateTime? ExpectedHarvestDate { get; set; }

        public List<string>? Tags { get; set; }
        public List<AgriFieldImageRequest>? Images { get; set; }
        public List<AgriFieldVideoRequest>? Videos { get; set; }
    }
}