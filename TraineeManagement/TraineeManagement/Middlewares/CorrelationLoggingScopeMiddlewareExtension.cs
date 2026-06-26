namespace TraineeManagement.Middlewares;

public static class CorrelationLogginfScopeMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationLoggingScope(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CorrelationLoggingScopeMiddleware>();
    }
}