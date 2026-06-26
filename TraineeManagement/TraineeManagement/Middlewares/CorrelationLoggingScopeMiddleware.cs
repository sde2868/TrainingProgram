using TraineeManagement.Constants;

namespace TraineeManagement.Middlewares;
public class CorrelationLoggingScopeMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<CorrelationLoggingScopeMiddleware> _logger;

    public CorrelationLoggingScopeMiddleware(
        RequestDelegate next,
        ILogger<CorrelationLoggingScopeMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Items[CorrelationConstants.ContextItemName] ?.ToString();

        using (_logger.BeginScope("CorrelationId:{CorrelationId}", correlationId))
        {
            await _next(context);
        }
    }
}