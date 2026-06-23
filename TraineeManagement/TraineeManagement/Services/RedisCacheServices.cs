using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using TraineeManagement.Interfaces;
using TraineeManagement.Models;

public class RedisCacheServices : ICacheService
{
    private static readonly JsonSerializerOptions _serializerOptions = new()
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true
    };

    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisCacheServices> _logger;
    private readonly RedisSettings _settings;

    public RedisCacheServices(
        IDistributedCache cache,
        ILogger<RedisCacheServices> logger,
        IOptions<RedisSettings> settings)
    {
        _cache = cache;
        _logger = logger;
        _settings = settings.Value;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            _logger.LogInformation($"GetAsync: getting value for key {key}.");
            var cachedValue =
                await _cache.GetStringAsync(key);

            if (string.IsNullOrEmpty(cachedValue))
                return default;

            return JsonSerializer.Deserialize<T>(
                cachedValue);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Redis read failed for key {Key}",
                key);

            return default;
        }
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiry = null)
    {
        try
        {
            _logger.LogInformation($"SetAsync: setting value for key {key}.");
            var options =
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow =
                        expiry ??
                        TimeSpan.FromMinutes(
                            _settings.DefaultTtlMinutes)
                };

            await _cache.SetStringAsync(
                key,
                JsonSerializer.Serialize(value),
                options);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Redis write failed for key {Key}",
                key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            _logger.LogInformation($"RemoveAsync: removing key {key}.");
            await _cache.RemoveAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Redis delete failed for key {Key}",
                key);
        }
    }
}