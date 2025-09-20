namespace GujaratClassified.API.Models.Entity
{
    public class AgriFieldLike
    {
        public int LikeId { get; set; }
        public int AgriFieldId { get; set; }
        public int UserId { get; set; }
        public string? ReactionType { get; set; } // LIKE, LOVE, INSPIRING, HELPFUL, WOW
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public string? UserName { get; set; }
        public string? UserProfileImage { get; set; }
    }
}
