namespace MeetingRoomBookingApi.Exceptions
{
    /// <summary>
    /// Heitetään kun varaus on ristiriidassa olemassa olevan tilan kanssa.
    /// Esimerkiksi: huone on jo varattu samalle ajanjaksolle.
    /// Kääntyy HTTP 409 Conflict -vastaukseksi.
    /// </summary>
    public class BookingConflictException : BookingException
    {
        public BookingConflictException(string message) : base(message)
        {
        }

        public BookingConflictException(string message, string errorCode, object errorDetails)
            : base(message, errorCode, errorDetails)
        {
        }
    }
}
