namespace GujaratClassified.API.Models.Entity
{
    public class UserOTP
    {
        public int OTPId { get; set; }
        public string Mobile { get; set; }
        public string OTPCode { get; set; }
        public string Purpose { get; set; } // REGISTER, LOGIN, FORGOT_PASSWORD
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int AttemptCount { get; set; } = 0;
    }
}
