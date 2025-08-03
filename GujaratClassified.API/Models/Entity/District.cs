namespace GujaratClassified.API.Models.Entity
{
    public class District
    {
        public int DistrictId { get; set; }
        public string DistrictNameGujarati { get; set; }
        public string DistrictNameEnglish { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
