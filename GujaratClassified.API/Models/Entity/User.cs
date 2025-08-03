namespace GujaratClassified.API.Models.Entity
{
    public class User
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public int DistrictId { get; set; }
        public int TalukaId { get; set; }
        public int VillageId { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;
        public bool IsPremium { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // Navigation properties
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
        public string? VillageName { get; set; }
    }
}
