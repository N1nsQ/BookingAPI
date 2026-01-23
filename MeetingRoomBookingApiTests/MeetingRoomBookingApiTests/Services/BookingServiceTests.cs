using FluentAssertions;
using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Exceptions;
using MeetingRoomBookingApi.Models;
using MeetingRoomBookingApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace MeetingRoomBookingApiTests.Services
{
    public class BookingServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ISystemTime> _mockTime;
        private readonly DateTime _fixedNow;
        private readonly BookingService _sut;
        private readonly BookingSettings _settings;

        public BookingServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            SeedTestData();

            _settings = new BookingSettings
            {
                MinBookingMinutes = 15,
                MaxBookingHours = 16,
                MaxBookingMonthsAhead = 6
            };

            var settingsOptions = Options.Create(_settings);

            _mockTime = new Mock<ISystemTime>();
            _fixedNow = new DateTime(2026, 1, 22, 12, 0, 0, DateTimeKind.Utc);
            _mockTime.Setup(t => t.Now).Returns(_fixedNow);

            _sut = new BookingService(_context, settingsOptions, _mockTime.Object);
        }

        private void SeedTestData()
        {
            var rooms = new List<MeetingRoom>
            {
                new() { Id = 1, Name = "Sali A" },
                new() { Id = 2, Name = "Sali B" }
            };

            _context.MeetingRooms.AddRange(rooms);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateBookingAsync_WhenEndTimeBeforeStartTime_ShouldThrowBookingValidationException()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(2),
                EndTime = _fixedNow.AddHours(1),
                AdditionalDetails = "Test"
            };

            // Act
            Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

            // Assert
            var exception = await act.Should().ThrowAsync<BookingValidationException>();
            exception.Which.Message.Should().Be("Aloitusajan on oltava ennen lopetusaikaa");
            exception.Which.ErrorCode.Should().Be("BOOKING_INVALID_TIME_RANGE");

            var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
            using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
            var root = jsonDoc.RootElement;

            root.GetProperty("requestedStart").GetDateTime().Should().Be(dto.StartTime);
            root.GetProperty("requestedEnd").GetDateTime().Should().Be(dto.EndTime);
        }

        [Fact]
        public async Task CreateBookingAsync_WhenEndTimeEqualsStartTime_ShouldThrowBookingValidationException()
        {
            // Arrange
            var sameTime = _fixedNow.AddHours(2);
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = sameTime,
                EndTime = sameTime,
                AdditionalDetails = "Test"
            };

            // Act
            Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

            // Assert
            await act.Should().ThrowAsync<BookingValidationException>()
                .WithMessage("Aloitusajan on oltava ennen lopetusaikaa");
        }

        [Fact]
        public async Task CreateBookingAsync_WhenEndTimeAfterStartTime_ShouldNotThrowTimeRangeException()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(2),
                AdditionalDetails = "Test"
            };

            // Act
            var result = await _sut.CreateBookingAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.StartTime.Should().Be(dto.StartTime);
            result.EndTime.Should().Be(dto.EndTime);

            var bookingInDb = await _context.Bookings.FirstOrDefaultAsync();
            bookingInDb.Should().NotBeNull();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-24)]
        [InlineData(-168)]
        public async Task CreateBookingAsync_WhenStartTimeInPast_WithDifferentOffsets_ShouldThrowException(int hoursOffset)
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(hoursOffset),
                EndTime = _fixedNow.AddHours(hoursOffset + 2),
                AdditionalDetails = "Test"
            };

            // Act
            var exception = await FluentActions
                .Invoking(() => _sut.CreateBookingAsync(dto))
                .Should()
                .ThrowAsync<BookingValidationException>();

            // Assert
            exception.Which.ErrorCode.Should().Be("BOOKING_IN_THE_PAST");
            exception.Which.Message.Should().Be("Varaus ei voi sijoittua menneisyyteen.");
        }

        [Fact]
        public async Task CreateBookingAsync_WhenStartTimeInPast_ShouldIncludeCorrectTimesInDetails()
        {
            // Arrange
            var pastTime = _fixedNow.AddHours(-2);
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = pastTime,
                EndTime = _fixedNow.AddHours(1),
                AdditionalDetails = "Test"
            };

            // Act
            Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

            // Assert
            var exception = await act.Should().ThrowAsync<BookingValidationException>();

            var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
            using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
            var root = jsonDoc.RootElement;

            root.GetProperty("requestedStart").GetDateTime().Should().Be(pastTime);
            root.GetProperty("currentTime").GetDateTime().Should().Be(_fixedNow);
        }

        [Fact]
        public async Task CreateBookingAsync_WhenStartTimeIsInFuture_ShouldCreateBookingSuccessfully()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(2),
                AdditionalDetails = "Test"
            };

            // Act
            var result = await _sut.CreateBookingAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.StartTime.Should().Be(_fixedNow.AddHours(1));
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(14)]
        public async Task CreateBookingAsync_WhenBookingTooShort_ShouldThrowBookingValidationException(int durationMinutes)
        {
            // Arrange
            var start = _fixedNow.AddHours(1);

            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = start,
                EndTime = start.AddMinutes(durationMinutes),
                AdditionalDetails = "Test"
            };

            // Act
            Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

            // Assert
            var exception = await act.Should().ThrowAsync<BookingValidationException>();
            exception.Which.ErrorCode.Should().Be("BOOKING_TOO_SHORT");
            exception.Which.Message.Should().Contain($"{_settings.MinBookingMinutes} minuuttia");
        }

        [Fact]
        public async Task CreateBookingAsync_WhenBookingExactlyMinimum_ShouldNotThrowShortException()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(1).AddMinutes(_settings.MinBookingMinutes),
                AdditionalDetails = "Test"
            };

            // Act
            var result = await _sut.CreateBookingAsync(dto);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateBookingAsync_WhenBookingTooLong_ShouldThrowBookingValidationException()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(27),
                AdditionalDetails = "Test"
            };

            // Act
            Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

            // Assert
            var exception = await act.Should().ThrowAsync<BookingValidationException>();
            exception.Which.ErrorCode.Should().Be("BOOKING_TOO_LONG");
            exception.Which.Message.Should().Contain($"{_settings.MaxBookingHours} tuntia");
        }

        [Fact]
        public async Task CreateBookingAsync_WhenBookingTooLong_ShouldIncludeDurationInDetails()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(21),
                AdditionalDetails = "Test"
            };

            // Act
            Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

            // Assert
            var exception = await act.Should().ThrowAsync<BookingValidationException>();

            var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
            using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
            var root = jsonDoc.RootElement;

            root.GetProperty("requestedHours").GetDouble().Should().Be(20);
            root.GetProperty("maxHours").GetInt32().Should().Be(_settings.MaxBookingHours);
        }

        [Fact]
        public async Task CreateBookingAsync_WhenBookingExactlyMaximum_ShouldNotThrowLongException()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(1 + _settings.MaxBookingHours),
                AdditionalDetails = "Test"
            };

            // Act
            var result = await _sut.CreateBookingAsync(dto);

            // Assert
            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData(25)]
        [InlineData(34)]
        [InlineData(100)]
        public async Task CreateBookingAsync_WhenBookingOverMaximum_WithDifferentDurations_ShouldThrowException(int hours)
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(1 + hours),
                AdditionalDetails = "Test"
            };

            // Act & Assert
            await FluentActions.Invoking(async () => await _sut.CreateBookingAsync(dto))
                .Should().ThrowAsync<BookingValidationException>()
                .Where(e => e.ErrorCode == "BOOKING_TOO_LONG");
        }

        [Theory]
        [InlineData(7)]
        [InlineData(12)]
        [InlineData(24)]
        public async Task CreateBookingAsync_WhenBookingTooFarInFuture_ShouldThrowBookingValidationException(int months)
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddMonths(months),
                EndTime = _fixedNow.AddMonths(months).AddHours(1),
                AdditionalDetails = "Test"
            };

            // Act
            Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

            // Assert
            var exception = await act.Should().ThrowAsync<BookingValidationException>();
            exception.Which.ErrorCode.Should().Be("BOOKING_TOO_FAR_IN_FUTURE");
            exception.Which.Message.Should().Contain($"{_settings.MaxBookingMonthsAhead} kuukauden");
        }

        [Fact]
        public async Task CreateBookingAsync_WhenBookingTooFarInFuture_ShouldIncludeDatesInDetails()
        {
            // Arrange
            var farFutureDate = _fixedNow.AddMonths(8);
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = farFutureDate,
                EndTime = farFutureDate.AddHours(1),
                AdditionalDetails = "Test"
            };

            // Act
            Func<Task> act = async () => await _sut.CreateBookingAsync(dto);

            // Assert
            var exception = await act.Should().ThrowAsync<BookingValidationException>();

            var detailsJson = System.Text.Json.JsonSerializer.Serialize(exception.Which.ErrorDetails);
            using var jsonDoc = System.Text.Json.JsonDocument.Parse(detailsJson);
            var root = jsonDoc.RootElement;

            DateOnly.FromDateTime(
                root.GetProperty("requestedStartDate").GetDateTime()
                ).Should().Be(DateOnly.FromDateTime(farFutureDate));


            DateOnly.Parse(
                root.GetProperty("maximumStartDate").GetString()!
            ).Should().Be(
                DateOnly.FromDateTime(_fixedNow).AddMonths(_settings.MaxBookingMonthsAhead)
            );
        }

        [Fact]
        public async Task CreateBookingAsync_WhenBookingExactlyAtMaximumFuture_ShouldNotThrowFarAheadException()
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddMonths(_settings.MaxBookingMonthsAhead),
                EndTime = _fixedNow.AddMonths(_settings.MaxBookingMonthsAhead).AddHours(1),
                AdditionalDetails = "Test"
            };

            // Act
            var result = await _sut.CreateBookingAsync(dto);

            // Assert
            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(6)]
        public async Task CreateBookingAsync_WhenBookingWithinAllowedFuture_ShouldSucceed(int months)
        {
            // Arrange
            var dto = new CreateBookingDto
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddMonths(months),
                EndTime = _fixedNow.AddMonths(months).AddHours(1),
                AdditionalDetails = "Test"
            };

            // Act
            var result = await _sut.CreateBookingAsync(dto);

            // Assert
            result.Should().NotBeNull();
            result.StartTime.Should().Be(_fixedNow.AddMonths(months));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        public async Task GetBookingAsync_WhenBookingNotFound_ShouldThrowNotFoundException(int bookingId)
        {
            // Act
            Func<Task> act = async () => await _sut.GetBookingAsync(bookingId);

            // Assert
            var exception = await act.Should().ThrowAsync<NotFoundException>();

            exception.Which.Message.Should().Be($"Varausta ID:llä {bookingId} ei löytynyt.");
        }

        [Fact]
        public async Task GetBookingAsync_WhenBookingExistsAmongOthers_ShouldReturnCorrectBookingDto()
        {
            // Arrange
            var bookings = new List<Booking>
        {
            new() {
                Id = 1,
                MeetingRoomId = 1,
                BookedBy = "User 1",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(2),
                AdditionalDetails = "First"
            },
            new() {
                Id = 2,
                MeetingRoomId = 2,
                BookedBy = "User 2",
                StartTime = _fixedNow.AddHours(3),
                EndTime = _fixedNow.AddHours(4),
                AdditionalDetails = "Second"
            }
        };

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetBookingAsync(2);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(2);
            result.MeetingRoomId.Should().Be(2);
            result.MeetingRoomName.Should().Be("Sali B");
            result.BookedBy.Should().Be("User 2");
            result.StartTime.Should().Be(_fixedNow.AddHours(3));
            result.EndTime.Should().Be(_fixedNow.AddHours(4));
            result.AdditionalDetails.Should().Be("Second");
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 2)]
        public async Task GetRoomBookingsAsync_WhenBookingsExistForRoom_ShouldReturnOnlyThatRoomsBookings(int roomId, int expectedCount)
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new() { MeetingRoomId = 1, BookedBy = "User A", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
                new() { MeetingRoomId = 1, BookedBy = "User B", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) },
                new() { MeetingRoomId = 2, BookedBy = "User C", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
                new() { MeetingRoomId = 2, BookedBy = "User D", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) }
            };

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetRoomBookingsAsync(roomId);

            // Assert
            result.Should().HaveCount(expectedCount);
            result.Should().OnlyContain(b => b.MeetingRoomId == roomId);
        }

        [Fact]
        public async Task GetRoomBookingsAsync_ShouldReturnBookingsOrderedByStartTime()
        {
            // Arrange
            var bookings = new List<Booking>
        {
            new() {
                MeetingRoomId = 1,
                BookedBy = "User 1",
                StartTime = _fixedNow.AddHours(5),
                EndTime = _fixedNow.AddHours(6)
            },
            new() {
                MeetingRoomId = 1,
                BookedBy = "User 2",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(2)
            },
            new() {
                MeetingRoomId = 1,
                BookedBy = "User 3",
                StartTime = _fixedNow.AddHours(3),
                EndTime = _fixedNow.AddHours(4)
            }
        };
            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _sut.GetRoomBookingsAsync(1)).ToList();

            // Assert
            result.Should().HaveCount(3);
            result[0].BookedBy.Should().Be("User 2");
            result[1].BookedBy.Should().Be("User 3");
            result[2].BookedBy.Should().Be("User 1");
            result.Should().BeInAscendingOrder(b => b.StartTime);
        }

        [Fact]
        public async Task GetRoomBookingsAsync_WhenRoomHasNoBookings_ShouldReturnEmptyList()
        {
            // Arrange
            var roomId = 1;

            // Act
            var result = await _sut.GetRoomBookingsAsync(roomId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetRoomBookingsAsync_WhenRoomDoesNotExist_ShouldThrowNotFoundException()
        {
            // Arrange
            var nonExistentRoomId = 999;

            // Act
            Func<Task> act = async () => await _sut.GetRoomBookingsAsync(nonExistentRoomId);

            // Assert
            var exception = await act.Should().ThrowAsync<NotFoundException>();
            exception.Which.Message.Should().Be($"Kokoushuonetta ID:llä {nonExistentRoomId} ei löydy.");
        }

        [Fact]
        public async Task GetRoomBookingsAsync_ShouldIncludeMeetingRoomNames()
        {
            // Arrange
            var booking = new Booking
            {
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(2)
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetRoomBookingsAsync(1);

            // Assert
            result.Should().HaveCount(1);
            result.First().MeetingRoomName.Should().Be("Sali A");
        }

        [Fact]
        public async Task GetAllBookingsAsync_WhenBookingsExist_ShouldReturnAllBookingsFromAllRooms()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new() { MeetingRoomId = 1, BookedBy = "User 1", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
                new() { MeetingRoomId = 1, BookedBy = "User 2", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) },
                new() { MeetingRoomId = 2, BookedBy = "User 3", StartTime = _fixedNow.AddHours(5), EndTime = _fixedNow.AddHours(6) }
            };

            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sut.GetAllBookingsAsync();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(b => b.MeetingRoomId == 1);
            result.Should().Contain(b => b.MeetingRoomId == 2);
        }

        [Fact]
        public async Task GetAllBookingsAsync_ShouldReturnBookingsOrderedByStartTime()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new() { MeetingRoomId = 1, BookedBy = "User C", StartTime = _fixedNow.AddHours(5), EndTime = _fixedNow.AddHours(6) },
                new() { MeetingRoomId = 2, BookedBy = "User A", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
                new() { MeetingRoomId = 1, BookedBy = "User B", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) }
            };
            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _sut.GetAllBookingsAsync()).ToList();

            // Assert
            result.Should().HaveCount(3);
            result[0].BookedBy.Should().Be("User A");
            result[1].BookedBy.Should().Be("User B");
            result[2].BookedBy.Should().Be("User C");
            result.Should().BeInAscendingOrder(b => b.StartTime);
        }

        [Fact]
        public async Task GetAllBookingsAsync_WhenNoBookings_ShouldReturnEmptyList()
        {
            // Act
            var result = await _sut.GetAllBookingsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllBookingsAsync_ShouldIncludeMeetingRoomNames()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new() { MeetingRoomId = 1, BookedBy = "User 1", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
                new() { MeetingRoomId = 2, BookedBy = "User 2", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) }
            };
            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _sut.GetAllBookingsAsync()).ToList();

            // Assert
            result[0].MeetingRoomName.Should().Be("Sali A");
            result[1].MeetingRoomName.Should().Be("Sali B");
        }

        [Fact]
        public async Task GetAllBookingsAsync_ShouldMapAllDtoProperties()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(2),
                AdditionalDetails = "Important meeting"
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            var result = (await _sut.GetAllBookingsAsync()).First();

            // Assert
            result.Id.Should().Be(1);
            result.MeetingRoomId.Should().Be(1);
            result.MeetingRoomName.Should().Be("Sali A");
            result.BookedBy.Should().Be("Test User");
            result.StartTime.Should().Be(_fixedNow.AddHours(1));
            result.EndTime.Should().Be(_fixedNow.AddHours(2));
            result.AdditionalDetails.Should().Be("Important meeting");
        }

        [Fact]
        public async Task DeleteBookingAsync_WhenBookingExists_ShouldDeleteBooking()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(2)
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var initialCount = await _context.Bookings.CountAsync();

            // Act
            await _sut.DeleteBookingAsync(1);

            // Assert
            var finalCount = await _context.Bookings.CountAsync();
            finalCount.Should().Be(initialCount - 1);

            var deletedBooking = await _context.Bookings.FindAsync(1);
            deletedBooking.Should().BeNull();
        }

        [Fact]
        public async Task DeleteBookingAsync_ShouldNotAffectOtherBookings()
        {
            // Arrange
            var bookings = new List<Booking>
            {
                new() { Id = 1, MeetingRoomId = 1, BookedBy = "User 1", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) },
                new() { Id = 2, MeetingRoomId = 1, BookedBy = "User 2", StartTime = _fixedNow.AddHours(3), EndTime = _fixedNow.AddHours(4) },
                new() { Id = 3, MeetingRoomId = 2, BookedBy = "User 3", StartTime = _fixedNow.AddHours(1), EndTime = _fixedNow.AddHours(2) }
            };
            _context.Bookings.AddRange(bookings);
            await _context.SaveChangesAsync();

            // Act
            await _sut.DeleteBookingAsync(2);

            // Assert
            var remainingBookings = await _context.Bookings.ToListAsync();
            remainingBookings.Should().HaveCount(2);
            remainingBookings.Should().Contain(b => b.Id == 1);
            remainingBookings.Should().Contain(b => b.Id == 3);
            remainingBookings.Should().NotContain(b => b.Id == 2);
        }

        [Fact]
        public async Task DeleteBookingAsync_WhenCalledMultipleTimes_SecondCallShouldThrowNotFoundException()
        {
            // Arrange
            var booking = new Booking
            {
                Id = 1,
                MeetingRoomId = 1,
                BookedBy = "Test User",
                StartTime = _fixedNow.AddHours(1),
                EndTime = _fixedNow.AddHours(2)
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            // Act
            await _sut.DeleteBookingAsync(1);

            // Assert
            Func<Task> act = async () => await _sut.DeleteBookingAsync(1);
            await act.Should().ThrowAsync<NotFoundException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(999)]
        public async Task DeleteBookingAsync_WhenBookingNotFound_ShouldThrowNotFoundException(int bookingId)
        {
            // Act
            Func<Task> act = async () => await _sut.DeleteBookingAsync(bookingId);

            // Assert
            var exception = await act.Should().ThrowAsync<NotFoundException>();
            exception.Which.Message.Should().Be($"Varausta ID:llä {bookingId} ei löydy.");
        }

    }
}
