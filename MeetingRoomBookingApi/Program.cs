using Microsoft.EntityFrameworkCore;
using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.Services;
using MeetingRoomBookingApi.Middleware;
using MeetingRoomBookingApi.Models;
using MeetingRoomBookingApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.Configure<BookingSettings>(
    builder.Configuration.GetSection("BookingSettings"));

// Lisää Entity Framework Core InMemory database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MeetingRoomDb"));

// Rekisteröi services
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISystemTime, SystemTime>();

// Lisää Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Alusta tietokanta seed-datalla
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();