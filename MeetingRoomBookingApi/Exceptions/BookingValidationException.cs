namespace MeetingRoomBookingApi.Exceptions
{
    /// <summary>
    /// Heitetään kun varauksen luonti epäonnistuu validointivirheen takia.
    /// Kääntyy HTTP 400 Bad Request -vastaukseksi.
    /// </summary>
    public class BookingValidationException : BookingException
    {
        public BookingValidationException(string message) : base(message)
        {
        }

        public BookingValidationException(string message, string errorCode) : base(message, errorCode) { }

        public BookingValidationException(string message, string errorCode, object errorDetails)
            : base(message, errorCode, errorDetails)
        {
        }
    }
}
