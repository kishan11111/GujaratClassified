namespace GujaratClassified.API.Models.Response
{
    public class PostListResponse
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public string Condition { get; set; }
        public string Status { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ViewCount { get; set; }
        public int FavoriteCount { get; set; }

        // Location info
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
        public string? VillageName { get; set; }

        // Category info
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }

        // Main image only
        public string? MainImageUrl { get; set; }

        // Computed properties
        public string LocationString => $"{VillageName}, {TalukaName}";
        public string PriceString => PriceType == "ON_CALL" ? "Price on Call" : $"₹{Price:N0}";
        public string TimeAgo
        {
            get
            {
                var timeSpan = DateTime.UtcNow - CreatedAt;
                if (timeSpan.Days > 0) return $"{timeSpan.Days}d ago";
                if (timeSpan.Hours > 0) return $"{timeSpan.Hours}h ago";
                if (timeSpan.Minutes > 0) return $"{timeSpan.Minutes}m ago";
                return "Just now";
            }
        }
    }
}
