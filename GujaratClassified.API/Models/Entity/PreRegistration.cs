namespace GujaratClassified.API.Models.Entity
{
    public class PreRegistration
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public int? DistrictId { get; set; }
        public int? TalukaId { get; set; }
        public int? VillageId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsConverted { get; set; } = false;
        public DateTime? ConvertedAt { get; set; }
    }
}
