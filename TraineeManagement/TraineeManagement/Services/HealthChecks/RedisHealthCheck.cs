using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace TraineeManagement.Services.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _config;

        public RedisHealthCheck(IConfiguration config)
        {
            _config = config;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var cs = _config["Redis:ConnectionString"];
            if (string.IsNullOrWhiteSpace(cs))
                return HealthCheckResult.Healthy("Redis not configured");

            try
            {
                var mux = await ConnectionMultiplexer.ConnectAsync(cs);
                var db = mux.GetDatabase();
                var pong = await db.PingAsync();
                return HealthCheckResult.Healthy($"Pong: {pong.TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(ex.Message);
            }
        }
    }
}
