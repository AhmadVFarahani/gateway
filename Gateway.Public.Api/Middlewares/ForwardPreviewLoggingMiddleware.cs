using Gateway.Application.Routes.Dtos;
using Microsoft.Extensions.Caching.Memory;
using System.Text.RegularExpressions;

namespace Gateway.Public.Api.Middlewares;

public class ForwardPreviewLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ForwardPreviewLoggingMiddleware> _logger;
    private readonly IMemoryCache _memory;

    public ForwardPreviewLoggingMiddleware(
        RequestDelegate next,
        ILogger<ForwardPreviewLoggingMiddleware> logger,
        IMemoryCache memory)
    {
        _next = next;
        _logger = logger;
        _memory = memory;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // ✅ Read route info from context safely
            if (context.Items.TryGetValue("RouteInfo", out var routeObj) && routeObj is RouteForwardDto routeInfo)
            {
                var routes = _memory.Get<List<RouteForwardDto>>("gateway:routes:forward");
                var dto = routes?.FirstOrDefault(r => r.Id == routeInfo.Id);

                if (dto != null)
                {
                    string sourcePath = Normalize(dto.Path);
                    string targetPath = Normalize(dto.TargetPath);
                    string requestPath = Normalize(context.Request.Path.Value ?? "/");
                    string query = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";

                    string targetUrl = BuildTargetUrl(dto.TargetBaseUrl, sourcePath, targetPath, requestPath, query);

                    _logger.LogInformation("🔁 [Forward] {Method} {From} → {To}",
                        context.Request.Method,
                        context.Request.Path + context.Request.QueryString,
                        targetUrl);

                    // Store resolved target for potential later use
                    context.Items["ResolvedTargetUrl"] = targetUrl;
                }
                else
                {
                    _logger.LogWarning("ForwardPreview: Route not found in cache for RouteId={RouteId}", routeInfo.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while building forward preview log.");
        }

        await _next(context);
    }

    // --- Helpers ---

    private static string Normalize(string path)
        => "/" + string.Join('/', (path ?? "/").Trim().Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries))
               .ToLowerInvariant();

    private static string BuildTargetUrl(string baseUrl, string sourceTemplate, string targetTemplate, string requestPath, string query)
    {
        var paramValues = ExtractParams(sourceTemplate, requestPath);

        var resolvedTargetPath = Regex.Replace(targetTemplate, "{([^}]+)}", m =>
        {
            var name = m.Groups[1].Value;
            return paramValues.TryGetValue(name, out var val) ? val : "";
        });

        var sep = baseUrl.EndsWith('/') ? "" : "/";
        var full = $"{baseUrl}{sep}{resolvedTargetPath.TrimStart('/')}";
        if (!string.IsNullOrEmpty(query)) full += query;
        return full;
    }

    private static Dictionary<string, string> ExtractParams(string template, string actual)
    {
        var tSeg = template.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);
        var aSeg = actual.Trim('/').Split('/', StringSplitOptions.RemoveEmptyEntries);

        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        for (int i = 0; i < tSeg.Length && i < aSeg.Length; i++)
        {
            var ts = tSeg[i];
            if (ts.StartsWith("{") && ts.EndsWith("}"))
            {
                var name = ts.Trim('{', '}');
                dict[name] = aSeg[i];
            }
        }
        return dict;
    }
}