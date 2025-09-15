using BabeNest_Backend.Exceptions;
using System.Net;
using System.Text.Json;

namespace BabeNest_Backend.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred: {ex.Message}");

                context.Response.ContentType = "application/json";

                var statusCode = ex switch
                {
                    NotFoundException => (int)HttpStatusCode.NotFound,       // 404
                    BadRequestException => (int)HttpStatusCode.BadRequest,   // 400
                    UnauthorizedException => (int)HttpStatusCode.Unauthorized, // 401
                    _ => (int)HttpStatusCode.InternalServerError             // 500
                };

                context.Response.StatusCode = statusCode;

                var response = new
                {
                    StatusCode = statusCode,
                    Message = ex.Message,
                    ErrorType = ex.GetType().Name
                };

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
            }
        }
    }
}
