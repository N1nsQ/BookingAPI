using System.ComponentModel.DataAnnotations;

namespace MeetingRoomBookingApi.DTOs
{
    public class CreateBookingDto
    {
        [Required]
        public int MeetingRoomId { get; set; }

        [Required]
        public string BookedBy { get; set; } = string.Empty;

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string? Description { get; set; }
    }
}