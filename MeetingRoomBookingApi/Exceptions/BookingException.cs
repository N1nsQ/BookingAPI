namespace MeetingRoomBookingApi.Exceptions
{
    public class BookingException : Exception
    {
        public object? ErrorDetails { get; set; }

        protected BookingException(string message) : base(message) { }

        protected BookingException(string message, object? errorDetails) : base(message)
        {
            ErrorDetails = errorDetails;
        }

    }
}
