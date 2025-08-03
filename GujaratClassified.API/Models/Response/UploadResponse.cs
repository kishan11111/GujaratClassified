namespace GujaratClassified.API.Models.Response
{
    public class UploadResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public long? FileSizeBytes { get; set; }
        public string? MimeType { get; set; }
        public List<string>? Errors { get; set; }
    }
}
