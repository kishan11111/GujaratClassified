using GujaratClassified.API.Models.Request;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(SendEmailRequest request);
        Task<bool> SendOTPEmailAsync(string toEmail, string toName, string otp);
        Task<bool> SendWelcomeEmailAsync(string toEmail, string userName);
        Task<bool> SendPostApprovalEmailAsync(string toEmail, string userName, string postTitle);
        Task<bool> SendPostRejectionEmailAsync(string toEmail, string userName, string postTitle, string reason);
    }
}
