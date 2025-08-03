using GujaratClassified.API.Models.Response;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IOTPService
    {
        Task<ApiResponse<OTPResponse>> SendOTPAsync(string mobile, string purpose);
        Task<ApiResponse<object>> VerifyOTPAsync(string mobile, string otpCode, string purpose);
        string GenerateOTP();
        Task<bool> SendSMSAsync(string mobile, string message);
    }
}
