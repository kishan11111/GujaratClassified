namespace GujaratClassified.API.Models.Entity
{
    public class Taluka
    {
        public int TalukaId { get; set; }
        public int DistrictId { get; set; }
        public string TalukaNameGujarati { get; set; }
        public string TalukaNameEnglish { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property (not stored in DB, used for joins)
        public string? DistrictName { get; set; }
    }
}
