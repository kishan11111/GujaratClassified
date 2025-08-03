namespace GujaratClassified.API.Models.Response
{
    public class AdminLoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public AdminProfileResponse Admin { get; set; }
    }
}
