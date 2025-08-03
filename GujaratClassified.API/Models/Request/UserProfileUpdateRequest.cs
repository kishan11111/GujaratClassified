using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class UserProfileUpdateRequest
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "District is required")]
        public int DistrictId { get; set; }

        [Required(ErrorMessage = "Taluka is required")]
        public int TalukaId { get; set; }

        [Required(ErrorMessage = "Village is required")]
        public int VillageId { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(@"^[MFO]$", ErrorMessage = "Gender must be M, F, or O")]
        public string? Gender { get; set; }

        public string? ProfileImage { get; set; }
    }
}
