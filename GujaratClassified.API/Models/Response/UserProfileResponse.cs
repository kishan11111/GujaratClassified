namespace GujaratClassified.API.Models.Response
{
    public class UserProfileResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Mobile { get; set; }
        public string? Email { get; set; }
        public int DistrictId { get; set; }
        public int TalukaId { get; set; }
        public int VillageId { get; set; }
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
        public string? VillageName { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public bool IsVerified { get; set; }
        public bool IsPremium { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Additional info for mobile app
        public string FullName => $"{FirstName} {LastName}";
        public string LocationString => $"{VillageName}, {TalukaName}, {DistrictName}";
    }
}
