using Microsoft.EntityFrameworkCore;
using MeetingRoomBookingApi.Models;

namespace MeetingRoomBookingApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<MeetingRoom> MeetingRooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed-data: Lisää muutama kokoushuone
            modelBuilder.Entity<MeetingRoom>().HasData(
                new MeetingRoom { Id = 1, Name = "Sali A", Capacity = 10 },
                new MeetingRoom { Id = 2, Name = "Sali B", Capacity = 6 },
                new MeetingRoom { Id = 3, Name = "Sali C", Capacity = 4 }
            );
        }
    }
}