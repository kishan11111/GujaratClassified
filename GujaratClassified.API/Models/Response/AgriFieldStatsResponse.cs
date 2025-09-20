namespace GujaratClassified.API.Models.Response
{
    public class AgriFieldStatsResponse
    {
        public int TotalFarmPosts { get; set; }
        public int ActiveFarmers { get; set; }
        public int TotalComments { get; set; }
        public int TotalDistrictsActive { get; set; }
        public List<TrendingTopicResponse>? TrendingTopics { get; set; }
        public List<TopFarmerResponse>? TopFarmers { get; set; }
        public List<CropStatsResponse>? CropStats { get; set; }
    }

    public class TrendingTopicResponse
    {
        public string Tag { get; set; }
        public int PostCount { get; set; }
    }

    public class TopFarmerResponse
    {
        public int UserId { get; set; }
        public string FarmerName { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsVerified { get; set; }
        public int TotalPosts { get; set; }
        public int TotalLikes { get; set; }
        public int TotalFollowers { get; set; }
        public string? DistrictName { get; set; }
    }

    public class CropStatsResponse
    {
        public string CropType { get; set; }
        public int PostCount { get; set; }
        public int FarmerCount { get; set; }
        public decimal? AvgFarmSize { get; set; }
    }
}