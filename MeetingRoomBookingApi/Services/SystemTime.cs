namespace MeetingRoomBookingApi.Services
{
    public class SystemTime : ISystemTime
    {
        public DateTime Now => DateTime.Now;
    }
}
