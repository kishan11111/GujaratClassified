using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Entity
{
    public class PostLike
    {
        public int LikeId { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties (not stored in DB, used for joins)
        public string? UserName { get; set; }
        public string? UserMobile { get; set; }
    }
}