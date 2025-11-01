namespace GujaratClassified.API.Models.Request
{
    public class LocalCardSearchRequest
    {
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? DistrictId { get; set; }
        public int? TalukaId { get; set; }
        public int? VillageId { get; set; }
        public string? SearchTerm { get; set; } // સર્ચ કરવા માટે
        public bool? IsVerified { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}