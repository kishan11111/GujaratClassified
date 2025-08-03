namespace GujaratClassified.API.Models.Response
{
    public class AdminProfileResponse
    {
        public int AdminId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsSuperAdmin { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
