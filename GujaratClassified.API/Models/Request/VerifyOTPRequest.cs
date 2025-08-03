using System.ComponentModel.DataAnnotations;

namespace GujaratClassified.API.Models.Request
{
    public class VerifyOTPRequest
    {
        [Required(ErrorMessage = "Mobile number is required")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be 10 digits")]
        public string Mobile { get; set; }

        [Required(ErrorMessage = "OTP is required")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "OTP must be 4-6 digits")]
        public string OTP { get; set; }

        [Required(ErrorMessage = "Purpose is required")]
        public string Purpose { get; set; } // REGISTER, LOGIN, FORGOT_PASSWORD
    }
}
