namespace GujaratClassified.API.Models.Response
{
    public class UserLoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public UserProfileResponse User { get; set; }
        public bool IsNewUser { get; set; } = false; // True if registered via OTP
    }
}
