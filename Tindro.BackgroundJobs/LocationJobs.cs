using Tindro.Application.Location.Services;
using Tindro.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Tindro.BackgroundJobs;

/// <summary>
/// Background job: Find crossed paths after location updates
/// </summary>
public class CrossedPathsJob
{
    private readonly CrossedPathsService _crossedPathsService;
    private readonly ILocationRepository _locationRepo;
    private readonly ILogger<CrossedPathsJob> _logger;

    public CrossedPathsJob(
        CrossedPathsService crossedPathsService,
        ILocationRepository locationRepo,
        ILogger<CrossedPathsJob> logger)
    {
        _crossedPathsService = crossedPathsService;
        _locationRepo = locationRepo;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("CrossedPaths job started");

        // This would typically be triggered per-user after location update
        // In production, use Hangfire or similar for scheduling

        _logger.LogInformation("CrossedPaths job completed");
    }

    /// <summary>
    /// Trigger for a specific user location update
    /// </summary>
    public async Task TriggerForUserAsync(Guid userId, string geohash)
    {
        try
        {
            await _crossedPathsService.FindCrossedPathsAsync(userId, geohash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding crossed paths for user {UserId}", userId);
        }
    }
}

/// <summary>
/// Background job: Delete expired location records (48 hour retention)
/// </summary>
public class LocationRetentionJob
{
    private readonly ILocationRepository _locationRepo;
    private readonly ILogger<LocationRetentionJob> _logger;

    public LocationRetentionJob(
        ILocationRepository locationRepo,
        ILogger<LocationRetentionJob> logger)
    {
        _locationRepo = locationRepo;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("LocationRetention job started");

        try
        {
            await _locationRepo.DeleteExpiredLocationsAsync();
            _logger.LogInformation("Expired locations deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expired locations");
        }
    }
}

/// <summary>
/// Background job: Delete expired crossed paths (7 day retention)
/// </summary>
public class CrossedPathsRetentionJob
{
    private readonly ICrossedPathRepository _crossedPathRepo;
    private readonly ILogger<CrossedPathsRetentionJob> _logger;

    public CrossedPathsRetentionJob(
        ICrossedPathRepository crossedPathRepo,
        ILogger<CrossedPathsRetentionJob> logger)
    {
        _crossedPathRepo = crossedPathRepo;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("CrossedPathsRetention job started");

        try
        {
            await _crossedPathRepo.DeleteExpiredCrossedPathsAsync();
            _logger.LogInformation("Expired crossed paths deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting expired crossed paths");
        }
    }
}
