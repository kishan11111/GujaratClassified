namespace GujaratClassified.API.Models.Response
{
    public class TalukaResponse
    {
        public int TalukaId { get; set; }
        public int DistrictId { get; set; }
        public string TalukaNameGujarati { get; set; }
        public string TalukaNameEnglish { get; set; }
        public string? DistrictName { get; set; }
        public bool IsActive { get; set; }
    }
}
