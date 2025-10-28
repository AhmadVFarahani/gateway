using Gateway.Application.Interfaces.Cache;
using Gateway.Application.Yarp;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Gateway.Public.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InternalCacheController : ControllerBase
{
    private readonly ILogger<InternalCacheController> _logger;
    private readonly ICacheRefresher _cacheRefresher;
    private readonly DynamicYarpConfigProvider _yarpProvider;

    public InternalCacheController(
        ILogger<InternalCacheController> logger,
        ICacheRefresher cacheRefresher,
        DynamicYarpConfigProvider yarpProvider)
    {
        _logger = logger;
        _cacheRefresher = cacheRefresher;
        _yarpProvider = yarpProvider;
    }

    /// <summary>
    /// Refresh in-memory and Redis caches and reload YARP routes.
    /// This endpoint is intentionally simple for testing (no auth).
    /// </summary>
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        var started = DateTime.UtcNow;
        _logger.LogInformation("Cache refresh requested at {TimeUtc}", started);

        try
        {
            // Refresh authorization-related caches (ApiKeys, Routes, Scopes, Policies)
            var t0 = DateTime.UtcNow;
            _logger.LogInformation("Refreshing authorization cache...");
            await _cacheRefresher.RefreshAuthorizationCacheAsync(cancellationToken);
            var t1 = DateTime.UtcNow;

            // Refresh business caches (Plans, Contracts, PlanRoutes, etc.)
            _logger.LogInformation("Refreshing business cache...");
            await _cacheRefresher.RefreshBusinessCacheAsync(cancellationToken);
            var t2 = DateTime.UtcNow;

            // Refresh YARP-related caches (Routes, Clusters, Destinations)
            _logger.LogInformation("Refreshing Yarp cache...");
            await _cacheRefresher.RefreshYarpCacheAsync(cancellationToken);
            var t3 = DateTime.UtcNow;

            // Tell Dynamic YARP provider to rebuild and notify YARP to reload
            _logger.LogInformation("Reloading YARP dynamic configuration...");
         
            _yarpProvider.Reload();
            var t4 = DateTime.UtcNow;

            var result = new
            {
                Status = "OK",
                StartedAtUtc = started,
                AuthorizationRefreshMs = (t1 - t0).TotalMilliseconds,
                BusinessRefreshMs = (t2 - t1).TotalMilliseconds,
                YarpCacheRefresh = (t3 - t2).TotalMilliseconds,
                YarpReloadMs = (t4 - t3).TotalMilliseconds,
                TotalMs = (t4 - started).TotalMilliseconds
            };

            _logger.LogInformation("Cache refresh completed successfully in {TotalMs} ms", result.TotalMs);
            return Ok(result);
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Cache refresh was cancelled.");
            return StatusCode(499, new { Status = "Cancelled" }); // 499 - client closed request (non-standard)
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cache refresh failed.");
            return StatusCode(500, new { Status = "Error", Message = ex.Message });
        }
    }
}