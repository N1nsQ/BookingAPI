namespace MeetingRoomBookingApi.Exceptions
{
    /// <summary>
    /// Heitetään kun pyydettyä resurssia (varausta tai huonetta) ei löydy.
    /// Kääntyy HTTP 404 Not Found -vastaukseksi.
    /// </summary>
    public class BookingNotFoundException : BookingException
    {
        public BookingNotFoundException(string message) : base(message)
        {
        }
    }
}