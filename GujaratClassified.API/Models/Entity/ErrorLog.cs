namespace GujaratClassified.API.Models.Entity
{
    public class ErrorLog
    {
        public int ErrorLogId { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public string StackTrace { get; set; }
        public string Source { get; set; }
        public string MethodName { get; set; }
        public string ControllerName { get; set; }
        public int? UserId { get; set; }
        public string UserMobile { get; set; }
        public string RequestPath { get; set; }
        public string RequestMethod { get; set; }
        public string RequestBody { get; set; }
        public string InnerException { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsResolved { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string ResolvedBy { get; set; }
        public string Notes { get; set; }
    }

    public class ApplicationLog
    {
        public int LogId { get; set; }
        public string LogLevel { get; set; }
        public string Message { get; set; }
        public string Category { get; set; }
        public string MethodName { get; set; }
        public int? UserId { get; set; }
        public string UserMobile { get; set; }
        public string RequestId { get; set; }
        public string SessionId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string AdditionalData { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public enum LogLevel
    {
        DEBUG,
        INFO,
        WARNING,
        ERROR,
        CRITICAL
    }
}