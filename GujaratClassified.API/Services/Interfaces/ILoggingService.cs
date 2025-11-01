using GujaratClassified.API.Models.Entity;

namespace GujaratClassified.API.Services.Interfaces
{
    public interface ILoggingService
    {
        Task<int> LogErrorAsync(Exception ex, string methodName = null, string controllerName = null,
            int? userId = null, string userMobile = null, string additionalInfo = null);

        Task<int> LogErrorAsync(string errorMessage, string errorType = null, string methodName = null,
            string controllerName = null, int? userId = null, string userMobile = null);

        Task<int> LogInfoAsync(string message, string category = null, string methodName = null,
            int? userId = null, string userMobile = null, object additionalData = null);

        Task<int> LogWarningAsync(string message, string category = null, string methodName = null,
            int? userId = null, string userMobile = null, object additionalData = null);

        Task<int> LogDebugAsync(string message, string category = null, string methodName = null,
            int? userId = null, string userMobile = null, object additionalData = null);

        Task<int> LogCriticalAsync(string message, string category = null, string methodName = null,
            int? userId = null, string userMobile = null, object additionalData = null);
    }
}
