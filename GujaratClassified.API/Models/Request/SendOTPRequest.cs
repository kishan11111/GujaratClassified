using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class SendOTPRequest
    {
        [Required(ErrorMessage = "Mobile number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid mobile number")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "Purpose is required")]
        public string Purpose { get; set; } // REGISTER, LOGIN, FORGOT_PASSWORD
    }
}
