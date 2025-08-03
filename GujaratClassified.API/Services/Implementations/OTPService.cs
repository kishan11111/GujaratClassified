using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;

namespace GujaratClassified.API.Services.Implementations
{
    public class OTPService : IOTPService
    {
        private readonly IOTPRepository _otpRepository;
        private readonly ILogger<OTPService> _logger;

        public OTPService(IOTPRepository otpRepository, ILogger<OTPService> logger)
        {
            _otpRepository = otpRepository;
            _logger = logger;
        }

        public async Task<ApiResponse<OTPResponse>> SendOTPAsync(string mobile, string purpose)
        {
            try
            {
                // Generate 6-digit OTP
                var otpCode = GenerateOTP();

                // Send SMS (implement your SMS service here)
                var smsMessage = $"Your Gujarat Classified OTP is: {otpCode}. Valid for 5 minutes. Do not share with anyone.";
                var smsSent = await SendSMSAsync(mobile, smsMessage);

                if (!smsSent)
                {
                    return ApiResponse<OTPResponse>.ErrorResponse("Failed to send SMS. Please try again.");
                }

                // Save OTP to database
                var otpSaved = await _otpRepository.SendOTPAsync(mobile, otpCode, purpose);

                if (!otpSaved)
                {
                    return ApiResponse<OTPResponse>.ErrorResponse("Failed to generate OTP. Please try again.");
                }

                var response = new OTPResponse
                {
                    Mobile = mobile,
                    Purpose = purpose,
                    ExpiryTime = DateTime.UtcNow.AddMinutes(5),
                    Message = $"OTP sent to +91-{mobile}",
                    CanResend = true
                };

                return ApiResponse<OTPResponse>.SuccessResponse(response, "OTP sent successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP to {Mobile}", mobile);
                return ApiResponse<OTPResponse>.ErrorResponse("An error occurred while sending OTP",
                    new List<string> { ex.Message });
            }
        }

        public async Task<ApiResponse<object>> VerifyOTPAsync(string mobile, string otpCode, string purpose)
        {
            try
            {
                var isValid = await _otpRepository.VerifyOTPAsync(mobile, otpCode, purpose);

                if (!isValid)
                {
                    return ApiResponse<object>.ErrorResponse("Invalid or expired OTP. Please try again.");
                }

                return ApiResponse<object>.SuccessResponse(null, "OTP verified successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying OTP for {Mobile}", mobile);
                return ApiResponse<object>.ErrorResponse("An error occurred while verifying OTP",
                    new List<string> { ex.Message });
            }
        }

        public string GenerateOTP()
        {
            // Generate 6-digit random OTP
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        public async Task<bool> SendSMSAsync(string mobile, string message)
        {
            try
            {
                // TODO: Implement your SMS service here
                // For now, we'll just log the OTP (for development)
                _logger.LogInformation("SMS to {Mobile}: {Message}", mobile, message);

                // In production, integrate with SMS services like:
                // - Twilio
                // - TextLocal
                // - MSG91
                // - AWS SNS
                // - Firebase Cloud Messaging

                // For development, always return true
                await Task.Delay(1000); // Simulate SMS sending delay
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send SMS to {Mobile}", mobile);
                return false;
            }
        }
    }
}