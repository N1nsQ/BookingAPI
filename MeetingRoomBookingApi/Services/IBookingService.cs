using MeetingRoomBookingApi.DTOs;

namespace MeetingRoomBookingApi.Services
{
    public interface IBookingService
    {
        Task<BookingDto> CreateBookingAsync(CreateBookingDto dto);
        Task<BookingDto> GetBookingAsync(int id);
        Task<IEnumerable<BookingDto>> GetRoomBookingsAsync(int roomId);
        Task<IEnumerable<BookingDto>> GetAllBookingsAsync();
        Task DeleteBookingAsync(int id);
    }
}