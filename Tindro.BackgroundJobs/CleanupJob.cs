using Hangfire;                           // or Quartz, whichever you're using
using Microsoft.Extensions.Logging;
using Tindro.Application.Common.Interfaces;
using Tindro.Application.Stories.Interfaces;
using Tindro.Domain.Match;
using Tindro.Domain.Stories;

namespace Tindro.BackgroundJobs;

public class CleanupJob
{
    private readonly IStoriesRepository _storyRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;       // optional
    private readonly IRedisService _redisService;           // optional - for cache invalidation
    private readonly ILogger<CleanupJob> _logger;

    // Recommended: inject repositories or services you need
    public CleanupJob(
        IStoriesRepository storyRepository,
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IRedisService redisService,
        ILogger<CleanupJob> logger)
    {
        _storyRepository = storyRepository;
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _redisService = redisService;
        _logger = logger;
    }

    // Main entry point - called by Hangfire/Quartz
    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting cleanup job at {Time}", DateTimeOffset.UtcNow);

        try
        {
            await CleanupExpiredStoriesAsync(cancellationToken);
            await CleanupOldSwipesAsync(cancellationToken);
            await CleanupExpiredBoostsAsync(cancellationToken);
            // await CleanupInactiveUsersAsync(cancellationToken);      // optional - be careful!

            _logger.LogInformation("Cleanup job completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cleanup job failed");
            // Optional: notify admin (email/slack) or requeue
            throw; // Let Hangfire retry or mark as failed
        }
    }

    private async Task CleanupExpiredStoriesAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var expiredStories = await _storyRepository.GetExpiredStoriesAsync(now, ct);

        if (!expiredStories.Any())
        {
            _logger.LogInformation("No expired stories found");
            return;
        }

        _logger.LogInformation("Found {Count} expired stories to delete", expiredStories.Count);

        foreach (var story in expiredStories)
        {
            // Optional: delete from cloud storage
            // await _storageService.DeleteFileAsync(story.MediaUrl);

            await _storyRepository.DeleteAsync(story.Id, ct);
        }

        // Optional: invalidate feed/story cache
        await _redisService.RemoveByPatternAsync("stories:*", ct);
    }

    private async Task CleanupOldSwipesAsync(CancellationToken ct)
    {
        const int retentionDays = 90;

        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        var deletedCount = await _matchRepository.DeleteOldSwipesAsync(cutoffDate, ct);

        _logger.LogInformation("Deleted {Count} old swipes older than {Days} days",
            deletedCount, retentionDays);
    }

    private async Task CleanupExpiredBoostsAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var expiredBoosts = await _matchRepository.GetExpiredBoostsAsync(now, ct);

        if (!expiredBoosts.Any()) return;

        _logger.LogInformation("Removing {Count} expired boosts", expiredBoosts.Count);

        foreach (var boost in expiredBoosts)
        {
            await _matchRepository.DeleteBoostAsync(boost.Id, ct);
        }
    }

    // Optional - use carefully (privacy/legal implications)
    // private async Task CleanupInactiveUsersAsync(CancellationToken ct)
    // {
    //     var cutoff = DateTime.UtcNow.AddMonths(-6);
    //     var inactiveUsers = await _userRepository.GetInactiveUsersAsync(cutoff, ct);
    //     ...
    // }
}