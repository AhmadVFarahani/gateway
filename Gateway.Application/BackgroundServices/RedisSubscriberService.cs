using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Microsoft.Extensions.Hosting;

namespace Gateway.Application.BackgroundServices;

public class RedisSubscriberService : BackgroundService
{
    private readonly ILogger<RedisSubscriberService> _logger;
    private readonly IConnectionMultiplexer _conn;
    private readonly IMemoryCache _memory;
    private readonly IConfiguration _cfg;

    public RedisSubscriberService(
        ILogger<RedisSubscriberService> logger,
        IConnectionMultiplexer conn,
        IMemoryCache memory,
        IConfiguration cfg)
    {
        _logger = logger;
        _conn = conn;
        _memory = memory;
        _cfg = cfg;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var instancePrefix = _cfg.GetSection("Cache")?["InstanceName"] ?? "AkamGateway:";
        var channel = "cache.refresh";

        var sub = _conn.GetSubscriber();
        await sub.SubscribeAsync(channel, async (chn, msg) =>
        {
            var pattern = msg.ToString();
            _logger.LogInformation("Cache refresh message received: {Pattern}", pattern);

            // Invalidate memory keys that match (simple startsWith)
            // Optional: maintain an index of keys put into memory to make this O(n) small.
            // Here, we rely on an index in Memory:
            if (_memory is MemoryCache mc)
            {
                var field = typeof(MemoryCache).GetField("_entries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var dict = field?.GetValue(mc) as System.Collections.IDictionary;
                if (dict != null)
                {
                    var toRemove = new List<object>();
                    foreach (System.Collections.DictionaryEntry e in dict)
                    {
                        var key = e.Key?.ToString() ?? "";
                        if (key.StartsWith(instancePrefix + pattern, StringComparison.OrdinalIgnoreCase) ||
                            pattern.EndsWith("*") && key.StartsWith(instancePrefix + pattern.TrimEnd('*'), StringComparison.OrdinalIgnoreCase))
                        {
                            toRemove.Add(e.Key!);
                        }
                    }
                    foreach (var k in toRemove) mc.Remove(k);
                    _logger.LogInformation("Invalidated {Count} memory keys for pattern {Pattern}", toRemove.Count, pattern);
                }
            }

            // Optionally: warm-up strategy (e.g., reload specific keys) can be added here.
            await Task.CompletedTask;
        });

        _logger.LogInformation("Subscribed to Redis channel: {Channel}", channel);
    }
}