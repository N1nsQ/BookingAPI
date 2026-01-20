using MeetingRoomBookingApi.DTOs;
using MeetingRoomBookingApi.Exceptions;
using System.Net;
using System.Text.Json;


namespace MeetingRoomBookingApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger; ;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Luo error response ja määritä status code
            var errorResponse = CreateErrorResponse(context, exception);

            // Logita virhe
            LogException(exception, errorResponse.StatusCode);

            // Aseta response
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorResponse.StatusCode;

            // Serialisoi ja palauta
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse, jsonOptions));
        }

        private ErrorResponseDto CreateErrorResponse(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            return exception switch
            {
                BookingValidationException validationEx => new ErrorResponseDto(
                    statusCode: (int)HttpStatusCode.BadRequest,
                    message: validationEx.Message,
                    traceId: traceId,
                    details: validationEx.ErrorDetails
                ),

                BookingNotFoundException notFoundEx => new ErrorResponseDto(
                    statusCode: (int)HttpStatusCode.NotFound,
                    message: notFoundEx.Message,
                    traceId: traceId
                ),

                BookingConflictException conflictEx => new ErrorResponseDto(
                    statusCode: (int)HttpStatusCode.Conflict,
                    message: conflictEx.Message,
                    traceId: traceId,
                    details: conflictEx.ErrorDetails
                ),

                // Kaikki muut virheet -> 500 Internal Server Error
                _ => new ErrorResponseDto(
                    statusCode: (int)HttpStatusCode.InternalServerError,
                    message: "Palvelimella tapahtui virhe. Yritä myöhemmin uudelleen.",
                    traceId: traceId
                )
            };
        }

        private void LogException(Exception exception, int statusCode)
        {
            // Logita eri tasolla riippuen virhetyypistä
            if (statusCode >= 500)
            {
                // Palvelinvirheet ovat vakavia
                _logger.LogError(exception, "Palvelinvirhe: {Message}", exception.Message);
            }
            else if (statusCode == 404)
            {
                // Not Found ei ole niin vakava
                _logger.LogWarning("Resurssia ei löytynyt: {Message}", exception.Message);
            }
            else
            {
                // Validointi- ja konflikttivirheet ovat informatiivisia
                _logger.LogInformation("Asiakasvirhe ({StatusCode}): {Message}", statusCode, exception.Message);
            }
        }

    }
}