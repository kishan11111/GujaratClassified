namespace GujaratClassified.API.Models.Response
{
    public class OTPResponse
    {
        public string Mobile { get; set; }
        public string Purpose { get; set; }
        public DateTime ExpiryTime { get; set; }
        public string Message { get; set; }
        public bool CanResend { get; set; } = true;
    }
}
