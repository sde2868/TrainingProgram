using Microsoft.AspNetCore.Http;
using TraineeManagement.Constants;

namespace TraineeManagement.Middlewares;

public class CorrelationIdMiddleware
{
    private const string HeaderName = CorrelationConstants.HeaderName;
    private readonly RequestDelegate _next;
    public CorrelationIdMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers[HeaderName]
                            .FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Items[CorrelationConstants.ContextItemName] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        await _next(context);
    }
}