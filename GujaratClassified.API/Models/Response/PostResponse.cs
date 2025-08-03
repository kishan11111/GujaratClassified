namespace GujaratClassified.API.Models.Response
{
    public class PostResponse
    {
        public int PostId { get; set; }
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string PriceType { get; set; }
        public string Condition { get; set; }
        public int DistrictId { get; set; }
        public int TalukaId { get; set; }
        public int VillageId { get; set; }
        public string? Address { get; set; }
        public string ContactMethod { get; set; }
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; }
        public bool IsSold { get; set; }
        public bool IsFeatured { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? SoldAt { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int ViewCount { get; set; }
        public int ContactCount { get; set; }
        public int FavoriteCount { get; set; }

        // Navigation properties
        public string? UserName { get; set; }
        public string? UserMobile { get; set; }
        public bool? UserVerified { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public string? DistrictName { get; set; }
        public string? TalukaName { get; set; }
        public string? VillageName { get; set; }
        public List<PostImageResponse>? Images { get; set; }
        public List<PostVideoResponse>? Videos { get; set; }

        // Computed properties
        public string LocationString => $"{VillageName}, {TalukaName}, {DistrictName}";
        public string PriceString => PriceType == "ON_CALL" ? "Price on Call" : $"₹{Price:N0}";
        public int DaysOld => (DateTime.UtcNow - CreatedAt).Days;
        public bool IsExpiringSoon => (ExpiryDate - DateTime.UtcNow).Days <= 7;
    }
}
