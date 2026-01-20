namespace MeetingRoomBookingApi.DTOs
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int MeetingRoomId { get; set; }
        public string MeetingRoomName { get; set; } = string.Empty;
        public string BookedBy { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? AdditionalDetails { get; set; }
    }
}