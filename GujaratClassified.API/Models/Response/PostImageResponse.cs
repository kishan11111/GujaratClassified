namespace GujaratClassified.API.Models.Response
{
    public class PostImageResponse
    {
        public int ImageId { get; set; }
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? OriginalFileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? MimeType { get; set; }
    }
}
