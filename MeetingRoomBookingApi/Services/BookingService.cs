using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Exceptions;
using MeetingRoomBookingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MeetingRoomBookingApi.Services
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        private const int MinBookingMinutes = 15;

        public BookingService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDto dto)
        {
            // Validoi: Aloitusajan täytyy olla ennen lopetusaikaa
            if (dto.StartTime >= dto.EndTime)
                throw new BookingValidationException("Aloitusajan on oltava ennen lopetusaikaa");

            // Validoi: Varaukset eivät voi sijoittua menneisyyteen
            if (dto.StartTime < DateTime.Now)
                throw new BookingValidationException("Varaus ei voi sijoittua menneisyyteen.");


            // Validoi: Varauksen minimipituus
            var duration = dto.EndTime - dto.StartTime;
            if (duration.TotalMinutes < MinBookingMinutes)
                throw new BookingValidationException($"Yritit tehdä {duration.TotalMinutes} minuutin varauksen. Varauksen minimipituus on {MinBookingMinutes} minuuttia.");


            // Tarkista että huone on olemassa
            var room = await _context.MeetingRooms.FindAsync(dto.MeetingRoomId) ?? throw new NotFoundException($"Kokoushuonetta ID:llä {dto.MeetingRoomId} ei löydy.");


            // Tarkista päällekkäisyydet
            var hasOverlap = await _context.Bookings
                .AnyAsync(b => b.MeetingRoomId == dto.MeetingRoomId &&
                             ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                              (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                              (dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));

            if (hasOverlap)
                throw new BookingConflictException("Huone on jo varattu kyseiselle ajanjaksolle.");


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
                .Include(b => b.MeetingRoom)
                .Where(b => b.MeetingRoomId == roomId)
                .OrderBy(b => b.StartTime)
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
                .ToListAsync();

            return bookings;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .OrderBy(b => b.StartTime)
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