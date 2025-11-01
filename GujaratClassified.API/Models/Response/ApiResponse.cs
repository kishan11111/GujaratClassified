using System.Text.Json.Serialization;

namespace GujaratClassified.API.Models.Response
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
        public List<string>? Errors { get; set; }
        public object? Pagination { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null
            };
        }

        public static ApiResponse<T> ErrorResponse(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default(T),
                Errors = errors ?? new List<string>()
            };
        }
    }
}
