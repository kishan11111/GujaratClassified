using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Services.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Text;

namespace GujaratClassified.API.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly string _toEmail;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Email configuration
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "Lokbazzar9099@gmail.com";
            _fromName = _configuration["EmailSettings:FromName"] ?? "LokBazzar";
            _toEmail = _configuration["EmailSettings:ToEmail"] ?? "lokbazzar9999@gmail.com";
            _smtpHost = _configuration["EmailSettings:SmtpHost"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "Lokbazzar9099@gmail.com";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "JayMataji@9099";
        }

        public async Task<bool> SendEmailAsync(SendEmailRequest request)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));

                // Determine recipient based on purpose
                var toEmail = GetRecipientEmail(request.Purpose);
                message.To.Add(new MailboxAddress("LokBazzar Team", toEmail));

                // Add Reply-To as sender's email
                message.ReplyTo.Add(new MailboxAddress(request.Name, request.Email));

                // Set subject
                message.Subject = GetEmailSubject(request);

                // Build email body based on purpose
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = GetEmailTemplate(request)
                };

                message.Body = bodyBuilder.ToMessageBody();

                // Send email
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation($"Email sent successfully - Purpose: {request.Purpose}, From: {request.Email}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email: {ex.Message}");
                return false;
            }
        }

        private string GetRecipientEmail(string purpose)
        {
            // All emails go to the configured ToEmail (lokbazzar9999@gmail.com)
            // For CONTACT_SELLER, you can later modify to send to actual seller's email
            return purpose.ToUpper() switch
            {
                "SUPPORT" => _toEmail,
                "FEEDBACK" => _toEmail,
                "CONTACT_SELLER" => _toEmail, // Can be changed to seller's email dynamically
                "INQUIRY" => _toEmail,
                "REPORT" => _toEmail,
                _ => _toEmail
            };
        }

        private string GetEmailSubject(SendEmailRequest request)
        {
            return request.Purpose.ToUpper() switch
            {
                "SUPPORT" => $"Support Request - {request.Subject}",
                "FEEDBACK" => $"User Feedback - {request.Subject}",
                "CONTACT_SELLER" => $"Product Inquiry - {request.Subject}",
                "INQUIRY" => $"General Inquiry - {request.Subject}",
                "REPORT" => $"Report Issue - {request.Subject}",
                _ => $"LokBazzar - {request.Subject}"
            };
        }

        private string GetEmailTemplate(SendEmailRequest request)
        {
            var sb = new StringBuilder();

            // Common header
            sb.Append(@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; color: #333; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background: linear-gradient(135deg, #004E89 0%, #FF6B35 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }
        .content { background: #f8f9fa; padding: 30px; border-radius: 0 0 10px 10px; }
        .info-row { margin: 15px 0; padding: 10px; background: white; border-radius: 5px; }
        .label { font-weight: bold; color: #004E89; }
        .message-box { background: white; padding: 20px; border-left: 4px solid #FF6B35; margin: 20px 0; }
        .footer { text-align: center; margin-top: 30px; color: #7F8C8D; font-size: 12px; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🏪 LokBazzar</h1>
            <p style='margin: 0;'>Gujarat's Leading Classified Marketplace</p>
        </div>
        <div class='content'>");

            // Purpose-specific content
            switch (request.Purpose.ToUpper())
            {
                case "SUPPORT":
                    sb.Append($@"
                        <h2 style='color: #FF6B35;'>🆘 Support Request</h2>
                        <div class='info-row'>
                            <span class='label'>From:</span> {request.Name}
                        </div>
                        <div class='info-row'>
                            <span class='label'>Email:</span> {request.Email}
                        </div>");
                    if (!string.IsNullOrEmpty(request.Phone))
                    {
                        sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Phone:</span> {request.Phone}
                        </div>");
                    }
                    sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Subject:</span> {request.Subject}
                        </div>
                        <div class='message-box'>
                            <p style='margin: 0;'><strong>Message:</strong></p>
                            <p>{request.Message.Replace("\n", "<br>")}</p>
                        </div>");
                    break;

                case "FEEDBACK":
                    sb.Append($@"
                        <h2 style='color: #FF6B35;'>💬 User Feedback</h2>
                        <div class='info-row'>
                            <span class='label'>From:</span> {request.Name}
                        </div>
                        <div class='info-row'>
                            <span class='label'>Email:</span> {request.Email}
                        </div>");
                    if (request.Rating.HasValue)
                    {
                        var stars = new string('⭐', request.Rating.Value);
                        sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Rating:</span> {stars} ({request.Rating.Value}/5)
                        </div>");
                    }
                    sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Subject:</span> {request.Subject}
                        </div>
                        <div class='message-box'>
                            <p style='margin: 0;'><strong>Feedback:</strong></p>
                            <p>{request.Message.Replace("\n", "<br>")}</p>
                        </div>");
                    break;

                case "CONTACT_SELLER":
                    sb.Append($@"
                        <h2 style='color: #FF6B35;'>📧 Product Inquiry</h2>
                        <div class='info-row'>
                            <span class='label'>Buyer Name:</span> {request.Name}
                        </div>
                        <div class='info-row'>
                            <span class='label'>Buyer Email:</span> {request.Email}
                        </div>");
                    if (!string.IsNullOrEmpty(request.Phone))
                    {
                        sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Buyer Phone:</span> {request.Phone}
                        </div>");
                    }
                    if (request.PostId.HasValue)
                    {
                        sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Post ID:</span> {request.PostId}
                        </div>");
                    }
                    if (!string.IsNullOrEmpty(request.PostTitle))
                    {
                        sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Product:</span> {request.PostTitle}
                        </div>");
                    }
                    sb.Append($@"
                        <div class='message-box'>
                            <p style='margin: 0;'><strong>Message:</strong></p>
                            <p>{request.Message.Replace("\n", "<br>")}</p>
                        </div>");
                    break;

                case "INQUIRY":
                    sb.Append($@"
                        <h2 style='color: #FF6B35;'>❓ General Inquiry</h2>
                        <div class='info-row'>
                            <span class='label'>From:</span> {request.Name}
                        </div>
                        <div class='info-row'>
                            <span class='label'>Email:</span> {request.Email}
                        </div>");
                    if (!string.IsNullOrEmpty(request.Phone))
                    {
                        sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Phone:</span> {request.Phone}
                        </div>");
                    }
                    sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Subject:</span> {request.Subject}
                        </div>
                        <div class='message-box'>
                            <p style='margin: 0;'><strong>Message:</strong></p>
                            <p>{request.Message.Replace("\n", "<br>")}</p>
                        </div>");
                    break;

                case "REPORT":
                    sb.Append($@"
                        <h2 style='color: #E74C3C;'>🚨 Report Issue</h2>
                        <div class='info-row'>
                            <span class='label'>Reported By:</span> {request.Name}
                        </div>
                        <div class='info-row'>
                            <span class='label'>Email:</span> {request.Email}
                        </div>");
                    if (request.PostId.HasValue)
                    {
                        sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Post ID:</span> {request.PostId}
                        </div>");
                    }
                    if (!string.IsNullOrEmpty(request.PageUrl))
                    {
                        sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Page URL:</span> <a href='{request.PageUrl}'>{request.PageUrl}</a>
                        </div>");
                    }
                    sb.Append($@"
                        <div class='info-row'>
                            <span class='label'>Issue:</span> {request.Subject}
                        </div>
                        <div class='message-box'>
                            <p style='margin: 0;'><strong>Details:</strong></p>
                            <p>{request.Message.Replace("\n", "<br>")}</p>
                        </div>");
                    break;

                default:
                    sb.Append($@"
                        <h2 style='color: #FF6B35;'>📬 New Message</h2>
                        <div class='info-row'>
                            <span class='label'>From:</span> {request.Name}
                        </div>
                        <div class='info-row'>
                            <span class='label'>Email:</span> {request.Email}
                        </div>
                        <div class='message-box'>
                            <p>{request.Message.Replace("\n", "<br>")}</p>
                        </div>");
                    break;
            }

            // Common footer
            sb.Append($@"
                        <div class='footer'>
                            <p>Sent from LokBazzar Website</p>
                            <p>📧 Reply to this email to contact the sender directly</p>
                            <p style='color: #FF6B35;'>© 2025 LokBazzar. All Rights Reserved.</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>");

            return sb.ToString();
        }

        // Additional helper methods for specific email types
        public async Task<bool> SendOTPEmailAsync(string toEmail, string toName, string otp)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = "Your LokBazzar OTP Code";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                .otp-box {{ background: #004E89; color: white; padding: 30px; text-align: center; border-radius: 10px; margin: 20px 0; }}
                                .otp-code {{ font-size: 48px; font-weight: bold; letter-spacing: 10px; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h1 style='color: #FF6B35;'>🏪 LokBazzar</h1>
                                <p>Hello {toName},</p>
                                <p>Your OTP code for LokBazzar verification is:</p>
                                <div class='otp-box'>
                                    <div class='otp-code'>{otp}</div>
                                </div>
                                <p>This OTP will expire in 10 minutes.</p>
                                <p>If you didn't request this code, please ignore this email.</p>
                                <hr>
                                <p style='color: #7F8C8D; font-size: 12px;'>© 2025 LokBazzar. All Rights Reserved.</p>
                            </div>
                        </body>
                        </html>"
                };

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending OTP email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string toEmail, string userName)
        {
            // Implementation for welcome email
            return await Task.FromResult(true);
        }

        public async Task<bool> SendPostApprovalEmailAsync(string toEmail, string userName, string postTitle)
        {
            // Implementation for post approval email
            return await Task.FromResult(true);
        }

        public async Task<bool> SendPostRejectionEmailAsync(string toEmail, string userName, string postTitle, string reason)
        {
            // Implementation for post rejection email
            return await Task.FromResult(true);
        }
    }
}
