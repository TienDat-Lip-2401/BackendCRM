namespace RedmineApp.Models.CommonModels
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public static ApiResponse Response(int statusCode, string message, object? data = null)
        {
            return new ApiResponse
            {
                StatusCode = statusCode,
                Message = message,
                Data = data
            };
        }
    }
}
