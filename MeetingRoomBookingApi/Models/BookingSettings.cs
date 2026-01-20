namespace MeetingRoomBookingApi.Models
{
    public class BookingSettings
    {
        public int MinBookingMinutes { get; set; }
        public int MaxBookingHours { get; set; }
        public int MaxBookingMonthsAhead { get; set; }
    }
}
