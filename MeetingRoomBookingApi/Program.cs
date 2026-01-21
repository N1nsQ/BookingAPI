using Microsoft.EntityFrameworkCore;
using MeetingRoomBookingApi.Data;
using MeetingRoomBookingApi.Services;
using MeetingRoomBookingApi.Middleware;
using MeetingRoomBookingApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<BookingSettings>(
    builder.Configuration.GetSection("BookingSettings"));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("MeetingRoomDb"));

builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ISystemTime, SystemTime>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();