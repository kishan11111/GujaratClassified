namespace GujaratClassified.API.DAL.Interfaces
{
    public interface IOTPRepository
    {
        Task<bool> SendOTPAsync(string mobile, string otpCode, string purpose);
        Task<bool> VerifyOTPAsync(string mobile, string otpCode, string purpose);
        Task<bool> IsOTPValidAsync(string mobile, string purpose);
    }
}
