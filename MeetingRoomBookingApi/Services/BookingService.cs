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
            var bookingDuration = dto.EndTime - dto.StartTime;

            // Validoi: Aloitusajan täytyy olla ennen lopetusaikaa
            if (dto.StartTime >= dto.EndTime)
                throw new BookingValidationException("Aloitusajan on oltava ennen lopetusaikaa");

            // Validoi: Varaukset eivät voi sijoittua menneisyyteen
            if (dto.StartTime < _time.Now)
                throw new BookingValidationException("Varaus ei voi sijoittua menneisyyteen.");


            // Validoi: Varauksen minimipituus
            if (bookingDuration.TotalMinutes < _settings.MinBookingMinutes)
                throw new BookingValidationException(
                    $"Varauksen minimipituus on {_settings.MinBookingMinutes} minuuttia.",
                    new
                    {
                        errorMessage = "Booking too short",
                        currentDuration = bookingDuration.TotalMinutes,
                        minimumDuration = _settings.MinBookingMinutes  
                    });

            // Validoi: Varauksen maximipituus
            if (bookingDuration.TotalHours > _settings.MaxBookingHours)
                throw new BookingValidationException(
                    $"Varauksen maximipituus on {_settings.MaxBookingHours} tuntia.",
                    new
                    {
                        errorMessage = "Booking too long",
                        currentDurationFormatted = $"{(int)bookingDuration.TotalDays} days, {bookingDuration.Hours} hours, {bookingDuration.Minutes} minutes",
                        maxHours = _settings.MaxBookingHours  
                    });

            // Validoi: Varaus max 6kk päähän nykyhetkestä
            var maxBookingDate = _time.Now.AddMonths(_settings.MaxBookingMonthsAhead);
            if (dto.StartTime > maxBookingDate)
                throw new BookingValidationException($"Voit tehdä varauksen enintään {_settings.MaxBookingMonthsAhead} kuukauden päähän nykyhetkestä.");

            // Tarkista että huone on olemassa
            var room = await _context.MeetingRooms.FindAsync(dto.MeetingRoomId) ?? throw new NotFoundException($"Kokoushuonetta ID:llä {dto.MeetingRoomId} ei löydy.");


            // Tarkista päällekkäisyydet
            var conflictingBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.MeetingRoomId == dto.MeetingRoomId &&
                             ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                              (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                              (dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));

            if (conflictingBooking != null)
            {
                throw new BookingConflictException(
                    "Huone on jo varattu kyseiselle ajanjaksolle.",
                    new
                    {
                        startTime = conflictingBooking.StartTime,
                        endTime = conflictingBooking.EndTime,
                        bookedBy = conflictingBooking.BookedBy
                    }
                );
            }

            // Luo varaus
            var booking = new Booking
            {
                MeetingRoomId = dto.MeetingRoomId,
                BookedBy = dto.BookedBy,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Description = dto.Description
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var resultDto = new BookingDto
            {
                Id = booking.Id,
                MeetingRoomId = booking.MeetingRoomId,
                MeetingRoomName = room.Name,
                BookedBy = booking.BookedBy,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Description = booking.Description
            };

            return resultDto;
        }

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
                Description = booking.Description
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
                    Description = b.Description
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
                    Description = b.Description
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