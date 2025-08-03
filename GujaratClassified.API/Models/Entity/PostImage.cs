namespace GujaratClassified.API.Models.Entity
{
    public class PostImage
    {
        public int ImageId { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public string? ImagePath { get; set; }
        public bool IsMain { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Image metadata
        public string? OriginalFileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? MimeType { get; set; }
    }
}
