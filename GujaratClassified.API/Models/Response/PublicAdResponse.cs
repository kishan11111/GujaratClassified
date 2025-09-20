namespace GujaratClassified.API.Models.Response
{
    public class PublicAdResponse
    {
        public int AdId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string AdType { get; set; }
        public string Position { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
        public string? TargetUrl { get; set; }
        public string? CompanyName { get; set; }
        public int Priority { get; set; }

        // No sensitive data like prices, contacts, analytics
    }
}
