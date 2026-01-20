namespace MeetingRoomBookingApi.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int MeetingRoomId { get; set; }
        public MeetingRoom MeetingRoom { get; set; } = null!;
        public string BookedBy { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Description { get; set; }
    }
}
