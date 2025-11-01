namespace GujaratClassified.API.Models.Entity
{
    public class Post
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? PriceType { get; set; } // FIXED, NEGOTIABLE, ON_CALL
        public string Condition { get; set; } // NEW, LIKE_NEW, GOOD, FAIR, POOR
        public int DistrictId { get; set; }
        public int TalukaId { get; set; }
        public int VillageId { get; set; }
        public string? Address { get; set; }
        public string ContactMethod { get; set; } // PHONE, CHAT, BOTH
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsSold { get; set; } = false;
        public bool IsFeatured { get; set; } = false;
        public string Status { get; set; } = "ACTIVE"; // ACTIVE, SOLD, EXPIRED, BLOCKED, PENDING
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SoldAt { get; set; }
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddDays(30);
        public int ViewCount { get; set; } = 0;
        public int ContactCount { get; set; } = 0;
        public int FavoriteCount { get; set; } = 0;

        // Navigation properties (not stored in DB, used for joins)
        public string? UserName { get; set; }
        public string? UserMobile { get; set; }
        public bool? UserVerified { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
        public string? VillageName { get; set; }
        public string? MainImageUrl { get; set; }
        public List<PostImage>? Images { get; set; }
        public List<PostVideo>? Videos { get; set; }
    }
}
