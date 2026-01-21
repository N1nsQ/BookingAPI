namespace MeetingRoomBookingApi.Exceptions
{
    public class BookingException : Exception
    {
        public string? ErrorCode { get; }
        public object? ErrorDetails { get; }

        protected BookingException(string message, string? errorCode, object? errorDetails) : base(message)
        {
            ErrorCode = errorCode;
            ErrorDetails = errorDetails;
        }

        public BookingException(string? message, string? errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        public BookingException(string? message) : base(message)
        {
        }
    }
}
