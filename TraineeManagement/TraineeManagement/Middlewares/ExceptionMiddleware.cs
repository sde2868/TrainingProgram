using System.Net;
using System.Text.Json;
using TraineeManagement.Exceptions;

namespace TraineeManagement.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred.");

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response already started, cannot handle exception.");
                    throw;
                }

                context.Response.Clear();
                context.Response.ContentType = "application/json";

                var statusCode = GetStatusCode(ex);
                context.Response.StatusCode = (int)statusCode;

                var response = new
                {
                    success = false,
                    statusCode = (int)statusCode,
                    message = GetMessage(ex),
                    // Show detailed error only in development
                    details = _env.IsDevelopment() ? ex.Message : null
                };

                var json = JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            }
        }

        private static HttpStatusCode GetStatusCode(Exception ex)
        {
            return ex switch
            {
                ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                KeyNotFoundException => HttpStatusCode.NotFound,
                FileNotFoundException => HttpStatusCode.NotFound,
                FileTooLargeException => HttpStatusCode.RequestEntityTooLarge, // 413
                InvalidOperationException =>  HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string GetMessage(
            Exception ex)
        {
            return ex switch
            {
                ArgumentException => ex.Message,
                InvalidOperationException => ex.Message,
                UnauthorizedAccessException => "Unauthorized access.",
                KeyNotFoundException => ex.Message,
                FileNotFoundException => ex.Message,
                FileTooLargeException => ex.Message,
                _ => "An unexpected error occurred. Please try again later."
            };
        }
    }
}