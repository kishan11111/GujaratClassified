namespace GujaratClassified.API.Models.Entity
{
    public class LocalCardImage
    {
        public int ImageId { get; set; }
        public int CardId { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}