namespace MeetingRoomBookingApi.DTOs
{
    public class ErrorResponseDto
    {
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public string? ErrorCode { get; set; }
        public string? TraceId { get; set; }
        public object? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public ErrorResponseDto()
        {
        }

        public ErrorResponseDto(int statusCode, string errorCode, string message, string? traceId = null, object? details = null)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
            Message = message;
            TraceId = traceId;
            Details = details;
        }
    }
}