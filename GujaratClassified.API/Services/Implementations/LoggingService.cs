using System.Data;
using System.Text.Json;
using Dapper;
using GujaratClassified.API.DAL;
using GujaratClassified.API.DAL.Interfaces;
using GujaratClassified.API.Models.Entity;
using GujaratClassified.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace GujaratClassified.API.Services.Implementations
{
    public class LoggingService : ILoggingService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggingService(IDbConnectionFactory connectionFactory, IHttpContextAccessor httpContextAccessor)
        {
            _connectionFactory = connectionFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> LogErrorAsync(Exception ex, string methodName = null, string controllerName = null,
            int? userId = null, string userMobile = null, string additionalInfo = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var httpContext = _httpContextAccessor.HttpContext;

                var parameters = new DynamicParameters();
                parameters.Add("@ErrorMessage", ex.Message);
                parameters.Add("@ErrorType", ex.GetType().FullName);
                parameters.Add("@StackTrace", ex.StackTrace);
                parameters.Add("@Source", ex.Source);
                parameters.Add("@MethodName", methodName);
                parameters.Add("@ControllerName", controllerName);
                parameters.Add("@UserId", userId);
                parameters.Add("@UserMobile", userMobile);
                parameters.Add("@RequestPath", httpContext?.Request?.Path.Value);
                parameters.Add("@RequestMethod", httpContext?.Request?.Method);

                // Get request body if it's a POST/PUT request
                string requestBody = null;
                if (httpContext?.Request != null && (httpContext.Request.Method == "POST" || httpContext.Request.Method == "PUT"))
                {
                    try
                    {
                        httpContext.Request.EnableBuffering();
                        httpContext.Request.Body.Position = 0;
                        using var reader = new StreamReader(httpContext.Request.Body);
                        requestBody = await reader.ReadToEndAsync();
                        httpContext.Request.Body.Position = 0;
                    }
                    catch { }
                }
                parameters.Add("@RequestBody", requestBody ?? additionalInfo);

                // Handle inner exception
                string innerExceptionDetails = null;
                if (ex.InnerException != null)
                {
                    innerExceptionDetails = $"Message: {ex.InnerException.Message}, StackTrace: {ex.InnerException.StackTrace}";
                }
                parameters.Add("@InnerException", innerExceptionDetails);

                var result = await connection.QuerySingleAsync<int>(
                    "SP_InsertErrorLog",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch
            {
                // If logging fails, we don't want to throw another exception
                return -1;
            }
        }

        public async Task<int> LogErrorAsync(string errorMessage, string errorType = null, string methodName = null,
            string controllerName = null, int? userId = null, string userMobile = null)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var httpContext = _httpContextAccessor.HttpContext;

                var parameters = new DynamicParameters();
                parameters.Add("@ErrorMessage", errorMessage);
                parameters.Add("@ErrorType", errorType ?? "Custom Error");
                parameters.Add("@StackTrace", null);
                parameters.Add("@Source", "Application");
                parameters.Add("@MethodName", methodName);
                parameters.Add("@ControllerName", controllerName);
                parameters.Add("@UserId", userId);
                parameters.Add("@UserMobile", userMobile);
                parameters.Add("@RequestPath", httpContext?.Request?.Path.Value);
                parameters.Add("@RequestMethod", httpContext?.Request?.Method);
                parameters.Add("@RequestBody", null);
                parameters.Add("@InnerException", null);

                var result = await connection.QuerySingleAsync<int>(
                    "SP_InsertErrorLog",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch
            {
                return -1;
            }
        }

        public async Task<int> LogInfoAsync(string message, string category = null, string methodName = null,
            int? userId = null, string userMobile = null, object additionalData = null)
        {
            return await LogAsync(Models.Entity.LogLevel.INFO.ToString(), message, category, methodName, userId, userMobile, additionalData);
        }

        public async Task<int> LogWarningAsync(string message, string category = null, string methodName = null,
            int? userId = null, string userMobile = null, object additionalData = null)
        {
            return await LogAsync(Models.Entity.LogLevel.WARNING.ToString(), message, category, methodName, userId, userMobile, additionalData);
        }

        public async Task<int> LogDebugAsync(string message, string category = null, string methodName = null,
            int? userId = null, string userMobile = null, object additionalData = null)
        {
            return await LogAsync(Models.Entity.LogLevel.DEBUG.ToString(), message, category, methodName, userId, userMobile, additionalData);
        }

        public async Task<int> LogCriticalAsync(string message, string category = null, string methodName = null,
            int? userId = null, string userMobile = null, object additionalData = null)
        {
            return await LogAsync(Models.Entity.LogLevel.CRITICAL.ToString(), message, category, methodName, userId, userMobile, additionalData);
        }

        private async Task<int> LogAsync(string logLevel, string message, string category, string methodName,
            int? userId, string userMobile, object additionalData)
        {
            try
            {
                using var connection = _connectionFactory.CreateConnection();
                var httpContext = _httpContextAccessor.HttpContext;

                var parameters = new DynamicParameters();
                parameters.Add("@LogLevel", logLevel);
                parameters.Add("@Message", message);
                parameters.Add("@Category", category);
                parameters.Add("@MethodName", methodName);
                parameters.Add("@UserId", userId);
                parameters.Add("@UserMobile", userMobile);
                parameters.Add("@RequestId", httpContext?.TraceIdentifier);
                parameters.Add("@SessionId", httpContext?.Session?.Id);
                parameters.Add("@IpAddress", GetClientIpAddress());
                parameters.Add("@UserAgent", httpContext?.Request?.Headers["User-Agent"].ToString());

                string additionalDataJson = null;
                if (additionalData != null)
                {
                    try
                    {
                        additionalDataJson = JsonSerializer.Serialize(additionalData);
                    }
                    catch { }
                }
                parameters.Add("@AdditionalData", additionalDataJson);

                var result = await connection.QuerySingleAsync<int>(
                    "SP_InsertApplicationLog",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return result;
            }
            catch
            {
                return -1;
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return null;

                // Try to get IP from X-Forwarded-For header (for reverse proxy scenarios)
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                // Try to get IP from X-Real-IP header
                var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                {
                    return realIp;
                }

                // Fall back to RemoteIpAddress
                return httpContext.Connection?.RemoteIpAddress?.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}