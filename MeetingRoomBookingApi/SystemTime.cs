using MeetingRoomBookingApi.Services;

namespace MeetingRoomBookingApi
{
    public class SystemTime : ISystemTime
    {
        public DateTime Now => DateTime.Now;
    }
}
