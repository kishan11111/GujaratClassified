namespace GujaratClassified.API.Models.Response
{
    public class VillageResponse
    {
        public int VillageId { get; set; }
        public int TalukaId { get; set; }
        public string VillageNameGujarati { get; set; }
        public string VillageNameEnglish { get; set; }
        public string? TalukaName { get; set; }
        public string? DistrictName { get; set; }
        public bool IsActive { get; set; }
    }
}
