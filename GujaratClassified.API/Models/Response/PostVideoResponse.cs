namespace GujaratClassified.API.Models.Response
{
    public class PostVideoResponse
    {
        public int VideoId { get; set; }
        public int PostId { get; set; }
        public string VideoUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? OriginalFileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? MimeType { get; set; }
        public int? DurationSeconds { get; set; }
    }
}
