using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Response;
using GujaratClassified.API.Services.Interfaces;
using static System.Net.WebRequestMethods;
using System.Text.Json;
using System.Web;

namespace GujaratClassified.API.Services.Implementations
{
    public class OTPService : IOTPService
    {
        private readonly IOTPRepository _otpRepository;
        private readonly ILogger<OTPService> _logger;
        private const string Fast2SmsApiKey =
    "NBdfTPU39SLEg2lkZnhqrQ75su6jYGxAtWvReVKC1i8DOHX0cpNsYRHzd2MIQlWgvno81itZ3A7heEOr";
        private static readonly HttpClient _http = new HttpClient();
        private const string Fast2SmsEndpoint = "https://www.fast2sms.com/dev/bulkV2";
        public OTPService(IOTPRepository otpRepository, ILogger<OTPService> logger)
        {
            _otpRepository = otpRepository;
            _logger = logger;
        }
        private sealed class Fast2SmsResponse
        {
            public bool @return { get; set; }
            public string request_id { get; set; }
            public List<string> message { get; set; }
        }
       

        // Main method is working below 
        public async Task<ApiResponse<OTPResponse>> SendOTPAsync(string mobile, string purpose)
        {
            try
            {
                // 1) Generate 4-digit OTP
                var otpCode = GenerateOTP(); // keep 4-digit (your method below)


                // 2) Send via Fast2SMS OTP route
                var smsSent = await SendOtpViaFast2SmsAsync(mobile, otpCode);

                if (!smsSent)
                    return ApiResponse<OTPResponse>.ErrorResponse("Failed to send SMS. Please try again.");

                // 3) Save OTP to DB
                var otpSaved = await _otpRepository.SendOTPAsync(mobile, otpCode, purpose);
                if (!otpSaved)
                    return ApiResponse<OTPResponse>.ErrorResponse("Failed to generate OTP. Please try again.");

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
                return ApiResponse<OTPResponse>.ErrorResponse(
                    "An error occurred while sending OTP",
                    new List<string> { ex.Message });
            }
        }


        private async Task<bool> SendOtpViaFast2SmsAsync(string mobile, string otp)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(mobile))
                    return false;

                // Fast2SMS DLT Route Configuration
                var requestBody = new
                {
                    route = "dlt",
                    sender_id = "LOKBZR",
                    message = "204080",  // DLT Template ID
                    variables_values = otp,  // OTP value to replace {#VAR#}
                    flash = 0,
                    numbers = mobile  // Mobile number without country code
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                using var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                // Create request with authorization header
                using var request = new HttpRequestMessage(HttpMethod.Post, Fast2SmsEndpoint);
                request.Headers.Add("authorization", Fast2SmsApiKey);
                request.Content = content;

                // Send HTTP POST request
                using var response = await _http.SendAsync(request);
                var body = await response.Content.ReadAsStringAsync();

                _logger.LogInformation("Fast2SMS DLT response: {Body}", body);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Fast2SMS request failed with status: {StatusCode}", response.StatusCode);
                    return false;
                }

                // Parse JSON response for success check
                var parsed = JsonSerializer.Deserialize<Fast2SmsResponse>(body,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return parsed != null && parsed.@return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fast2SMS OTP send failed for {Mobile}", mobile);
                return false;
            }
        }



        public async Task<ApiResponse<object>> VerifyOTPAsync(string mobile, string otpCode, string purpose)
        {
            _logger.LogInformation("OTPService.VerifyOTPAsync - Started for Mobile: {Mobile}, Purpose: {Purpose}, OTP: {OTP}",
                mobile, purpose, otpCode);

            try
            {
                _logger.LogInformation("Calling repository to verify OTP in database");

                var isValid = await _otpRepository.VerifyOTPAsync(mobile, otpCode, purpose);

                _logger.LogInformation("Repository returned result: {IsValid} for mobile: {Mobile}", isValid, mobile);

                if (!isValid)
                {
                    _logger.LogWarning("OTP validation failed - Mobile: {Mobile}, Purpose: {Purpose}, OTP: {OTP}",
                        mobile, purpose, otpCode);
                    return ApiResponse<object>.ErrorResponse("Invalid or expired OTP. Please try again.");
                }

                _logger.LogInformation("OTP verified successfully for mobile: {Mobile}, Purpose: {Purpose}",
                    mobile, purpose);
                return ApiResponse<object>.SuccessResponse(null, "OTP verified successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR in OTPService.VerifyOTPAsync for Mobile: {Mobile}. Exception: {ExceptionType}, Message: {Message}, StackTrace: {StackTrace}",
                    mobile, ex.GetType().Name, ex.Message, ex.StackTrace);

                return ApiResponse<object>.ErrorResponse(
                    "An error occurred while verifying OTP",
                    new List<string> { ex.Message });
            }
        }
        public string GenerateOTP()
        {
            // Generate 4-digit random OTP
            var random = new Random();
            return random.Next(1000, 9999).ToString();
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