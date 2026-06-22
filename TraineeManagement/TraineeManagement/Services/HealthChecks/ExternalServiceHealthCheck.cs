using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using System.Net.Http;

namespace TraineeManagement.Services.HealthChecks
{
    public class ExternalServiceHealthCheck : IHealthCheck
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public ExternalServiceHealthCheck(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var url = _config["Health:ExternalHealthUrl"];
            if (string.IsNullOrWhiteSpace(url))
            {
                // No external URL configured — treat as non-critical healthy
                return HealthCheckResult.Healthy("No external health URL configured");
            }

            try
            {
                using var resp = await _http.GetAsync(url, cancellationToken);
                if (resp.IsSuccessStatusCode)
                    return HealthCheckResult.Healthy();

                return HealthCheckResult.Unhealthy($"External service returned {(int)resp.StatusCode}");
            }
            catch (OperationCanceledException)
            {
                return HealthCheckResult.Unhealthy("External health check timed out");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }
    }
}
