using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Models;

namespace MeetingRoomBookingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private const int MinBookingMinutes = 15;

        // POST: api/Bookings
        [HttpPost]
        public async Task<ActionResult<BookingDto>> CreateBooking(CreateBookingDto dto)
        {
            // Validoi: Aloitusajan täytyy olla ennen lopetusaikaa
            if (dto.StartTime >= dto.EndTime)
            {
                return BadRequest("Aloitusajan täytyy olla ennen lopetusaikaa.");
            }

            // Validoi: Varaukset eivät voi sijoittua menneisyyteen
            if (dto.StartTime < DateTime.Now)
            {
                return BadRequest("Varaus ei voi sijoittua menneisyyteen.");
            }

            // Validoi: Varauksen minimipituus
            var duration = dto.EndTime - dto.StartTime;
            if (duration.TotalMinutes < MinBookingMinutes)
            {
                return BadRequest(new
                {
                    error = "BookingTooShort",
                    message = $"Varauksen minimipituus on {MinBookingMinutes} minuuttia.",
                    currentDuration = $"{duration.TotalMinutes} minuuttia",
                    minimumDuration = $"{MinBookingMinutes} minuuttia"
                });
            }

            // Tarkista että huone on olemassa
            var room = await _context.MeetingRooms.FindAsync(dto.MeetingRoomId);
            if (room == null)
            {
                return NotFound($"Kokoushuonetta ID:llä {dto.MeetingRoomId} ei löydy.");
            }

            // Tarkista päällekkäisyydet
            var hasOverlap = await _context.Bookings
                .AnyAsync(b => b.MeetingRoomId == dto.MeetingRoomId &&
                             ((dto.StartTime >= b.StartTime && dto.StartTime < b.EndTime) ||
                              (dto.EndTime > b.StartTime && dto.EndTime <= b.EndTime) ||
                              (dto.StartTime <= b.StartTime && dto.EndTime >= b.EndTime)));

            if (hasOverlap)
            {
                return Conflict("Huone on jo varattu kyseiselle ajanjaksolle.");
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

            return CreatedAtAction(nameof(GetBooking), new { id = booking.Id }, resultDto);
        }

        // GET: api/Bookings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetBooking(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.MeetingRoom)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

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

        // GET: api/Bookings/room/1
        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetRoomBookings(int roomId)
        {
            var room = await _context.MeetingRooms.FindAsync(roomId);
            if (room == null)
            {
                return NotFound($"Kokoushuonetta ID:llä {roomId} ei löydy.");
            }

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

            return Ok(bookings);
        }

        // DELETE: api/Bookings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound($"Varausta ID:llä {id} ei löydy.");
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Bookings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
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

            return Ok(bookings);
        }
    }
}