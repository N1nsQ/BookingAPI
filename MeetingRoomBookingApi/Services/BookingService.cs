using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Exceptions;
using MeetingRoomBookingApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace MeetingRoomBookingApi.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private readonly BookingSettings _settings;
        private readonly ISystemTime _time;

        public BookingService(ApplicationDbContext context, IOptions<BookingSettings> settings, ISystemTime time)
        {
            _context = context;
            _settings = settings.Value;
            _time = time;
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
        {
            ValidateTimeRange(dto);
            ValidateNotInPast(dto);
            ValidateBookingDuration(dto);
            ValidateMaxBookingDate(dto);

            var room = await GetMeetingRoomOrThrowAsync(dto.MeetingRoomId);

            await EnsureNoConflictsAsync(dto);

            var booking = CreateBookingEntity(dto);

            await SaveBookingAsync(booking);

            return MapToDto(booking, room);
        }

        private static void ValidateTimeRange(CreateBookingDto dto)
        {
            if (dto.StartTime >= dto.EndTime)
                throw new BookingValidationException(
                    "Aloitusajan on oltava ennen lopetusaikaa",
                    "BOOKING_INVALID_TIME_RANGE",
                    new
                    {
                        requestedStart = dto.StartTime,
                        requestedEnd = dto.EndTime
                    });
        }

        private void ValidateNotInPast(CreateBookingDto dto)
        {
            if (dto.StartTime < _time.Now)
                throw new BookingValidationException(
                    "Varaus ei voi sijoittua menneisyyteen.",
                    "BOOKING_IN_THE_PAST",
                    new
                    {
                        requestedStart = dto.StartTime,
                        currentTime = _time.Now
                    });
        }

        private void ValidateBookingDuration(CreateBookingDto dto)
        {
            var bookingDuration = dto.EndTime - dto.StartTime;

            if (bookingDuration.TotalMinutes < _settings.MinBookingMinutes)
                throw new BookingValidationException(
                    $"Varauksen minimipituus on {_settings.MinBookingMinutes} minuuttia.",
                    "BOOKING_TOO_SHORT",
                    new
                    {
                        requestedMinutes = bookingDuration.TotalMinutes,
                        minimumMinutes = _settings.MinBookingMinutes
                    });

            if (bookingDuration.TotalHours > _settings.MaxBookingHours)
                throw new BookingValidationException(
                    $"Varauksen maximipituus on {_settings.MaxBookingHours} tuntia.",
                    "BOOKING_TOO_LONG",
                    new
                    {
                        requestedHours = bookingDuration.TotalHours,
                        maxHours = _settings.MaxBookingHours
                    });
        }

        private void ValidateMaxBookingDate(CreateBookingDto dto)
        {
            var today = DateOnly.FromDateTime(_time.Now);
            var maxDate = today.AddMonths(_settings.MaxBookingMonthsAhead);
            var requestedDate = DateOnly.FromDateTime(dto.StartTime);

            if (requestedDate > maxDate)
                throw new BookingValidationException(
                    $"Voit tehdä varauksen enintään {_settings.MaxBookingMonthsAhead} kuukauden päähän nykyhetkestä.",
                    "BOOKING_TOO_FAR_IN_FUTURE",
                    new
                    {
                        requestedStartDate = requestedDate,
                        maximumStartDate = maxDate
                    });
        }

        private async Task<MeetingRoom> GetMeetingRoomOrThrowAsync(int roomId)
        {
            return await _context.MeetingRooms.FindAsync(roomId)
                ?? throw new NotFoundException($"Kokoushuonetta ID:llä {roomId} ei löydy.");
        }

        private async Task EnsureNoConflictsAsync(CreateBookingDto dto)
        {
            var conflict = await _context.Bookings
                .FirstOrDefaultAsync(b =>
                    b.MeetingRoomId == dto.MeetingRoomId &&
                    dto.StartTime < b.EndTime &&
                    dto.EndTime > b.StartTime);

            if (conflict != null)
                throw new BookingConflictException(
                    "Huone on jo varattu kyseiselle ajanjaksolle.",
                    "BOOKING_TIME_CONFLICT",
                    new
                    {
                        startTime = conflict.StartTime,
                        endTime = conflict.EndTime,
                        bookedBy = conflict.BookedBy
                    });
        }

        private static Booking CreateBookingEntity(CreateBookingDto dto) => new()
        {
            MeetingRoomId = dto.MeetingRoomId,
            BookedBy = dto.BookedBy,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            AdditionalDetails = dto.AdditionalDetails
        };

        private async Task SaveBookingAsync(Booking booking)
        {
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        private static BookingDto MapToDto(Booking booking, MeetingRoom room) => new()
        {
            Id = booking.Id,
            MeetingRoomId = booking.MeetingRoomId,
            MeetingRoomName = room.Name,
            BookedBy = booking.BookedBy,
            StartTime = booking.StartTime,
            EndTime = booking.EndTime,
            AdditionalDetails = booking.AdditionalDetails
        };

        public async Task<BookingDto> GetBookingAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .FirstOrDefaultAsync(b => b.Id == id) ?? throw new NotFoundException($"Varausta ID:llä {id} ei löytynyt.");

            var dto = new BookingDto
            {
                Id = booking.Id,
                MeetingRoomId = booking.MeetingRoomId,
                MeetingRoomName = booking.MeetingRoom.Name,
                BookedBy = booking.BookedBy,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                AdditionalDetails = booking.AdditionalDetails
            };

            return dto;
        }

        public async Task<IEnumerable<BookingDto>> GetRoomBookingsAsync(int roomId)
        {
            var room = await _context.MeetingRooms.FindAsync(roomId) ?? throw new NotFoundException($"Kokoushuonetta ID:llä {roomId} ei löydy.");

            var bookings = await _context.Bookings
                
                .Where(b => b.MeetingRoomId == roomId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    MeetingRoomId = b.MeetingRoomId,
                    MeetingRoomName = b.MeetingRoom.Name,
                    BookedBy = b.BookedBy,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    AdditionalDetails = b.AdditionalDetails
                })
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            return bookings;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    MeetingRoomId = b.MeetingRoomId,
                    MeetingRoomName = b.MeetingRoom.Name,
                    BookedBy = b.BookedBy,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    AdditionalDetails = b.AdditionalDetails
                })
                .OrderBy(b => b.StartTime)
                .ToListAsync();

            return bookings;
        }

        public async Task DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id) ?? throw new NotFoundException($"Varausta ID:llä {id} ei löydy.");
            _context.Bookings.Remove(booking);

            await _context.SaveChangesAsync();

        }

    }

}