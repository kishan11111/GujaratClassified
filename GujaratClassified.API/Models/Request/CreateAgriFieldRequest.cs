using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class CreateAgriFieldRequest
    {
        [Required(ErrorMessage = "Farm name is required")]
        [StringLength(200, ErrorMessage = "Farm name cannot exceed 200 characters")]
        public string FarmName { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(300, ErrorMessage = "Title cannot exceed 300 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "District is required")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Taluka is required")]
        public int TalukaId { get; set; }

        [Required(ErrorMessage = "Village is required")]
        public int VillageId { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Crop type is required")]
        [StringLength(100, ErrorMessage = "Crop type cannot exceed 100 characters")]
        public string CropType { get; set; }

        [Required(ErrorMessage = "Farming method is required")]
        [StringLength(50, ErrorMessage = "Farming method cannot exceed 50 characters")]
        public string FarmingMethod { get; set; }

        [Required(ErrorMessage = "Season is required")]
        [StringLength(50, ErrorMessage = "Season cannot exceed 50 characters")]
        public string Season { get; set; }

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

    public class AgriFieldImageRequest
    {
        [Required]
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public string? ImageType { get; set; }
        public bool IsMain { get; set; } = false;
        public int SortOrder { get; set; } = 0;
    }

    public class AgriFieldVideoRequest
    {
        [Required]
        public string VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Caption { get; set; }
        public string? VideoType { get; set; }
        public int SortOrder { get; set; } = 0;
        public int? DurationSeconds { get; set; }
    }
}