using Tindro.Application.Location.Dtos;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Location.Services;

public class CrossedPathsService
{
    private readonly ICrossedPathRepository _crossedPathRepo;
    private readonly ILocationRepository _locationRepo;
    private readonly ILocationPrivacyRepository _privacyRepo;

    public CrossedPathsService(
        ICrossedPathRepository crossedPathRepo,
        ILocationRepository locationRepo,
        ILocationPrivacyRepository privacyRepo)
    {
        _crossedPathRepo = crossedPathRepo;
        _locationRepo = locationRepo;
        _privacyRepo = privacyRepo;
    }

    /// <summary>
    /// Background job: Find users who crossed paths within 30 min window
    /// Triggered after location update
    /// </summary>
    public async Task FindCrossedPathsAsync(Guid userId, string geohash)
    {
        // Get all users at same geohash
        var nearbyLocations = await _locationRepo.GetNearbyLocationsAsync(geohash, precision: 6);

        foreach (var location in nearbyLocations.Where(l => l.UserId != userId))
        {
            // Check if within 30 minute window
            var timeDiff = DateTime.UtcNow.Subtract(location.UpdatedAt).TotalMinutes;
            if (timeDiff > 30)
                continue;

            // Check if already recorded
            if (await _crossedPathRepo.ExistsCrossedPathAsync(userId, location.UserId))
                continue;

            // Privacy check: other user has location enabled
            var privacy = await _privacyRepo.GetPrivacySettingsAsync(location.UserId);
            if (privacy?.IsLocationEnabled != true || privacy.IsPaused)
                continue;

            // Create crossed path record
            await _crossedPathRepo.CreateCrossedPathAsync(userId, location.UserId, geohash);
        }
    }

    /// <summary>
    /// Get crossed paths for a user
    /// </summary>
    public async Task<CrossedPathsResponse> GetCrossedPathsAsync(Guid userId)
    {
        var crossedPaths = await _crossedPathRepo.GetCrossedPathsForUserAsync(userId);

        var response = new CrossedPathsResponse
        {
            Paths = new(),
            TotalCount = crossedPaths.Count
        };

        foreach (var path in crossedPaths)
        {
            var otherUserId = path.User1Id == userId ? path.User2Id : path.User1Id;

            var dto = new CrossedPathDto
            {
                UserId = otherUserId,
                Username = $"User{otherUserId.ToString().Substring(0, 8)}",
                ProfilePhoto = "", // TODO: Get from profile
                Age = 25, // TODO: Get from profile
                CrossedAt = FuzzyTime(path.CrossedAt),
                Location = "Somewhere" // TODO: Get city from geohash
            };

            response.Paths.Add(dto);
        }

        return response;
    }

    /// <summary>
    /// Fuzzy timestamp for privacy
    /// </summary>
    private string FuzzyTime(DateTime crossedAt)
    {
        var diff = DateTime.UtcNow.Subtract(crossedAt);

        return diff switch
        {
            TimeSpan when diff.TotalMinutes < 60 => "Less than an hour ago",
            TimeSpan when diff.TotalHours < 24 => "Today",
            TimeSpan when diff.TotalDays < 7 => "This week",
            _ => "Recently"
        };
    }
}
