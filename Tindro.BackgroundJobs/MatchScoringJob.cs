using Hangfire;
using Microsoft.Extensions.Logging;
using Tindro.Application.Common.Interfaces;
using Tindro.Domain.Match;
using Tindro.Domain.Users;
using Tindro.Shared.Enums;

namespace Tindro.BackgroundJobs;

public class MatchScoringJob
{
    private readonly IUserRepository _userRepository;
    private readonly IMatchRepository _matchRepository;
    private readonly IRedisService _redis;           // very important for caching scores
    private readonly ILogger<MatchScoringJob> _logger;

    private const int BATCH_SIZE = 500;
    private const string SCORE_CACHE_KEY_PREFIX = "match:score:user:";

    public MatchScoringJob(
        IUserRepository userRepository,
        IMatchRepository matchRepository,
        IRedisService redis,
        ILogger<MatchScoringJob> logger)
    {
        _userRepository = userRepository;
        _matchRepository = matchRepository;
        _redis = redis;
        _logger = logger;
    }

    // Main entry point
    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Starting match scoring job — {Time}", DateTimeOffset.UtcNow);

        try
        {
            // 1. Update global popularity scores (Elo-like or custom formula)
            await UpdateUserPopularityScoresAsync(ct);

            // 2. Pre-calculate & cache top candidates for active users
            await RefreshTopCandidateScoresForActiveUsersAsync(ct);

            // 3. Optional: decay old scores / penalize inactivity
            await ApplyInactivityDecayAsync(ct);

            _logger.LogInformation("Match scoring job completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Match scoring job failed");
            throw; // Let Hangfire handle retries
        }
    }

    private async Task UpdateUserPopularityScoresAsync(CancellationToken ct)
    {
        // Get users who had activity recently
        var activeUsers = await _userRepository.GetRecentlyActiveUsersAsync(
            lastActiveAfter: DateTime.UtcNow.AddDays(-14),
            take: 5000,
            ct);

        int updated = 0;

        foreach (var user in activeUsers)
        {
            // Very simplified example formula — in reality much more complex
            double newScore = CalculatePopularityScore(user);

            user.UpdatePopularityScore(newScore);
            await _userRepository.UpdateAsync(user, ct);

            // Cache the score (very fast access in feed generation)
            await _redis.SetAsync(
                $"{SCORE_CACHE_KEY_PREFIX}{user.Id}",
                newScore,
                TimeSpan.FromHours(6),
                ct);

            updated++;

            if (updated % 100 == 0)
                _logger.LogInformation("Updated {Updated}/{Total} user popularity scores", updated, activeUsers.Count);
        }

        _logger.LogInformation("Popularity scores updated for {Count} users", updated);
    }

    private async Task RefreshTopCandidateScoresForActiveUsersAsync(CancellationToken ct)
    {
        // Only refresh for users who were online in last 24–48h
        var onlineUsers = await _userRepository.GetOnlineUsersAsync(TimeSpan.FromHours(36), ct);

        foreach (var user in onlineUsers)
        {
            // Get potential candidates (already excluding swiped, blocked, etc.)
            var candidates = await _userRepository.GetPotentialMatchCandidatesAsync(
                user,
                limit: 200,  // we score top 200, but feed usually shows 20–30
                ct);

            var scoredCandidates = new List<(string CandidateId, double Score)>();

            foreach (var candidate in candidates)
            {
                double score = await CalculateCompatibilityScore(user, candidate);
                scoredCandidates.Add((candidate.Id.ToString(), score));
            }

            // Sort descending & keep top N
            var topCandidates = scoredCandidates
                .OrderByDescending(x => x.Score)
                .Take(100)
                .ToList();

            // Cache as sorted list (very important for fast feed)
            await _redis.SetAsync(
                $"feed:candidates:{user.Id}",
                topCandidates.Select(x => $"{x.CandidateId}:{x.Score:F3}").ToArray(),
                TimeSpan.FromHours(4),
                ct);
        }

        _logger.LogInformation("Refreshed candidate scores for {Count} active users", onlineUsers.Count);
    }

    private async Task ApplyInactivityDecayAsync(CancellationToken ct)
    {
        // Very simple decay: reduce score for users inactive > 7 days
        var decayed = await _userRepository.ApplyInactivityDecayAsync(
            inactiveSince: DateTime.UtcNow.AddDays(-7),
            decayFactor: 0.92,   // ~8% penalty per day-ish
            ct);

        _logger.LogInformation("Applied inactivity decay to {Count} users", decayed);
    }

    // ──────────────────────────────────────────────────────────────
    //          Very simplified scoring functions (expand heavily!)
    // ──────────────────────────────────────────────────────────────

    private double CalculatePopularityScore(User user)
    {
        // Example factors (weights are tunable)
        double score = 50; // base

        score += user.LikeCountLast30Days * 2.5;
        score += user.MatchCountLast30Days * 10;
        score += user.MessageCountSentLast30Days * 1.8;
        score += user.PhotoCount * 4;
        score += user.HasBio ? 15 : 0;
        score += user.IsVerified ? 30 : 0;
        score += user.HasActiveSubscription ? 40 : 0;

        // Decay by inactivity
        var daysInactive = (DateTime.UtcNow - user.LastActive).TotalDays;
        score *= Math.Pow(0.97, daysInactive);

        return Math.Clamp(score, 0, 1000);
    }

    private async Task<double> CalculateCompatibilityScore(User current, User candidate)
    {
        // Simplified compatibility: prefer age matches and small boosts for shared interests
        double score = 0;

        // Age preference match (application uses Min/Max naming)
        if (current.Profile != null && candidate.Profile != null)
        {
            if (current.Profile.MinAgePreference <= candidate.Age && current.Profile.MaxAgePreference >= candidate.Age)
                score += 35;

            if (current.Profile.Interests != null && candidate.Profile.Interests != null)
            {
                var common = current.Profile.Interests.Intersect(candidate.Profile.Interests).Count();
                score += common * 6;
            }
        }

        // Boost multiplier if candidate has active boosts
        if (await HasActiveBoostAsync(candidate.Id.ToString()))
            score *= 2.0;

        return Math.Clamp(score, 0, 200);
    }

    private async Task<bool> HasActiveBoostAsync(string userId)
    {
        var boosts = await _matchRepository.GetActiveBoostsForUserAsync(userId, DateTime.UtcNow);
        return boosts.Any();
    }
}