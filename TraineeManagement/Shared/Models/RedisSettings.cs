namespace TraineeManagement.Models;

public class RedisSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public int DefaultTtlMinutes { get; set; }
}