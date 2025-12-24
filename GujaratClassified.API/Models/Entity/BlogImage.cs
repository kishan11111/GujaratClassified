namespace GujaratClassified.API.Models.Entity
{
    public class BlogImage
    {
        public int ImageId { get; set; }
        public int BlogId { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
