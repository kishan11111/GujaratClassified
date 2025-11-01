using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class NearbyCardsRequest
    {
        [Required(ErrorMessage = "Latitude is required")]
        public decimal Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        public decimal Longitude { get; set; }

        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public decimal RadiusKm { get; set; } = 10; // Default 10 km radius
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}