namespace GujaratClassified.API.Models.Entity
{
    public class AgriFieldImage
    {
        public int AgriImageId { get; set; }
        public int AgriFieldId { get; set; }
        public string ImageUrl { get; set; }
        public string? ImagePath { get; set; }
        public bool IsMain { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public string? Caption { get; set; }
        public string? ImageType { get; set; } // Crop, Soil, Equipment, Farmer, Harvest
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Image metadata
        public string? OriginalFileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? MimeType { get; set; }
    }
}
