namespace GujaratClassified.API.Models.Response
{
    public class PostStatsResponse
    {
        public int TotalPosts { get; set; }
        public int ActivePosts { get; set; }
        public int SoldPosts { get; set; }
        public int ExpiredPosts { get; set; }
        public int FeaturedPosts { get; set; }
        public int TotalViews { get; set; }
        public int TotalContacts { get; set; }
        public int TotalFavorites { get; set; }
        public decimal AveragePrice { get; set; }
        public DateTime? LastPostDate { get; set; }
    }
}
