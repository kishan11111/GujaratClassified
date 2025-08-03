namespace GujaratClassified.API.Models.Entity
{
    public class PostVideo
    {
        public int VideoId { get; set; }
        public int PostId { get; set; }
        public string VideoUrl { get; set; }
        public string? VideoPath { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Video metadata
        public string? OriginalFileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? MimeType { get; set; }
        public int? DurationSeconds { get; set; }
    }
}
