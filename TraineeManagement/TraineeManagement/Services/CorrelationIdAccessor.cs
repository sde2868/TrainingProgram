using Microsoft.AspNetCore.Http;
using TraineeManagement.Constants;
public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CorrelationIdAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCorrelationId()
    {
        var correlationId = _httpContextAccessor.HttpContext?
                            .Items[CorrelationConstants.ContextItemName]
                            ?.ToString();
        return correlationId ?? Guid.NewGuid().ToString();
    }
}