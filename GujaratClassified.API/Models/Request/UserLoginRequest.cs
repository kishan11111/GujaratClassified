using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Mobile number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
        public string Password { get; set; }
    }
}
