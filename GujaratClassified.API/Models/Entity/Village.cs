namespace GujaratClassified.API.Models.Entity
{
    public class Village
    {
        public int VillageId { get; set; }
        public int TalukaId { get; set; }
        public string VillageNameGujarati { get; set; }
        public string VillageNameEnglish { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (not stored in DB, used for joins)
        public string? TalukaName { get; set; }
        public string? DistrictName { get; set; }
    }
}
