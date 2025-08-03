using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class AdminProfileUpdateRequest
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Mobile is required")]
        [StringLength(15, ErrorMessage = "Mobile cannot exceed 15 characters")]
        [Phone(ErrorMessage = "Invalid mobile number format")]
        public string Mobile { get; set; }

        public string? ProfileImage { get; set; }
    }
}
