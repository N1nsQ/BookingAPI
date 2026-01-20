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

        public BookingValidationException(string message, object errorDetails)
            : base(message, errorDetails)
        {
        }
    }
}
