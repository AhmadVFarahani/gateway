namespace Gateway.Public.Api.Middlewares;

public static class InternalRequestHelper
{
    // Define all prefixes that are considered "internal" or non-user routes
    private static readonly string[] InternalPrefixes =
    {
        "/internal",
        "/api/internal",
        "/api/internalcache",
        "/health",
        "/metrics"
    };

    public static bool IsInternal(HttpContext context)
    {
        var path = context.Request.Path.ToString();

        return InternalPrefixes.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));

    }
}
