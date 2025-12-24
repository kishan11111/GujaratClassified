using GujaratClassified.API.Models.Request;
using GujaratClassified.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GujaratClassified.API.Controllers
{
    [ApiController]
    [Route("api/email")]
    [Produces("application/json")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Send email for various purposes (Support, Feedback, Contact Seller, etc.)
        /// </summary>
        /// <remarks>
        /// Purpose options:
        /// - SUPPORT: For support requests
        /// - FEEDBACK: For user feedback and ratings
        /// - CONTACT_SELLER: For contacting seller about a post
        /// - INQUIRY: For general inquiries
        /// - REPORT: For reporting issues or inappropriate content
        /// </remarks>
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Invalid request data",
                        errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                    });
                }

                // Validate purpose
                var validPurposes = new[] { "SUPPORT", "FEEDBACK", "CONTACT_SELLER", "INQUIRY", "REPORT" };
                if (!validPurposes.Contains(request.Purpose.ToUpper()))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = $"Invalid purpose. Must be one of: {string.Join(", ", validPurposes)}"
                    });
                }

                var result = await _emailService.SendEmailAsync(request);

                if (result)
                {
                    _logger.LogInformation($"Email sent successfully - Purpose: {request.Purpose}, From: {request.Email}");

                    return Ok(new
                    {
                        success = true,
                        message = "Email sent successfully! We will get back to you soon."
                    });
                }
                else
                {
                    _logger.LogError($"Failed to send email - Purpose: {request.Purpose}, From: {request.Email}");

                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Failed to send email. Please try again later or contact us directly."
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SendEmail: {ex.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An error occurred while sending the email"
                });
            }
        }

        /// <summary>
        /// Test email configuration
        /// </summary>
        [HttpGet("test")]
        public IActionResult TestEmail()
        {
            return Ok(new
            {
                success = true,
                message = "Email service is configured and ready",
                supportedPurposes = new[] { "SUPPORT", "FEEDBACK", "CONTACT_SELLER", "INQUIRY", "REPORT" }
            });
        }
    }
}
