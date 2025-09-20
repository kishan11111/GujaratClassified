namespace GujaratClassified.API.Models.Entity
{
    public class AgriFieldFollow
    {
        public int FollowId { get; set; }
        public int AgriFieldId { get; set; }
        public int FollowerUserId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public string? FollowerName { get; set; }
        public string? FollowerProfileImage { get; set; }
        public bool? FollowerVerified { get; set; }
    }
}
