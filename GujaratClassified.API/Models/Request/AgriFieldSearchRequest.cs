namespace GujaratClassified.API.Models.Request
{
    public class AgriFieldSearchRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchKeyword { get; set; }
        public int? DistrictId { get; set; }
        public int? TalukaId { get; set; }
        public int? VillageId { get; set; }
        public string? CropType { get; set; }
        public string? FarmingMethod { get; set; }
        public string? Season { get; set; }
        public string? SoilType { get; set; }
        public string? WaterSource { get; set; }
        public decimal? MinFarmSize { get; set; }
        public decimal? MaxFarmSize { get; set; }
        public string? SortBy { get; set; } = "recent"; // recent, popular, liked, followed
        public bool? FeaturedOnly { get; set; } = false;
        public List<string>? Tags { get; set; }
    }
}