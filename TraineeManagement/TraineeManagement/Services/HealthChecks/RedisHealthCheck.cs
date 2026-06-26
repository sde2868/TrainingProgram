using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace TraineeManagement.Services.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConfiguration _config;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public RedisHealthCheck(IConfiguration config, IConnectionMultiplexer connectionMultiplexer)
        {
            _config = config;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var cs = _config["Redis:ConnectionString"];
            if (string.IsNullOrWhiteSpace(cs))
                return HealthCheckResult.Healthy("Redis not configured");

            try
            {
                var db = _connectionMultiplexer.GetDatabase();
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
