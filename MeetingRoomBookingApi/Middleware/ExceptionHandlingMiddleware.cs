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
            var errorResponse = CreateErrorResponse(context, exception);

            LogException(exception, errorResponse.StatusCode);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorResponse.StatusCode;

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
                    errorCode: validationEx.ErrorCode ?? "VALIDATION_ERROR",
                    traceId: traceId,
                    details: validationEx.ErrorDetails
                ),

                NotFoundException notFoundEx => new ErrorResponseDto(
                    statusCode: (int)HttpStatusCode.NotFound,
                    message: notFoundEx.Message,
                    errorCode: "RESOURCE_NOT_FOUND",
                    traceId: traceId
                ),

                BookingConflictException conflictEx => new ErrorResponseDto(
                    statusCode: (int)HttpStatusCode.Conflict,
                    message: conflictEx.Message,
                    errorCode: conflictEx.ErrorCode ?? "BOOKING_CONFLICT",
                    traceId: traceId,
                    details: conflictEx.ErrorDetails
                ),

                _ => new ErrorResponseDto(
                    statusCode: (int)HttpStatusCode.InternalServerError,
                    message: "Palvelimella tapahtui virhe. Yritä myöhemmin uudelleen.",
                    errorCode: "INTERNAL_ERROR",
                    traceId: traceId
                )
            };
        }

        private void LogException(Exception exception, int statusCode)
        {
            if (statusCode >= 500)
            {
                _logger.LogError(exception, "Palvelinvirhe: {Message}", exception.Message);
            }
            else if (statusCode == 404)
            {
                _logger.LogWarning("Resurssia ei löytynyt: {Message}", exception.Message);
            }
            else
            {
                _logger.LogInformation("Asiakasvirhe ({StatusCode}): {Message}", statusCode, exception.Message);
            }
        }

    }
}