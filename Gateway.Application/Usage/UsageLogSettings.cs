namespace Gateway.Application.Usage;

/// <summary>
/// Configuration options for the usage log queue.
/// </summary>
public class UsageLogSettings
{
    public int QueueCapacity { get; set; } = 20000;
    public int BatchSize { get; set; } = 500;
    public int FlushIntervalSeconds { get; set; } = 2;
}