using Tindro.Application.Location.Dtos;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Location.Services;

/// <summary>
/// Generates UI-safe map positions with aggregated user counts
/// No GPS coordinates in response - only zones
/// </summary>
public class MapCardService
{
    private readonly ILocationRepository _locationRepo;
    private readonly ILocationPrivacyRepository _privacyRepo;
    private readonly IRedisService _redis;

    public MapCardService(
        ILocationRepository locationRepo,
        ILocationPrivacyRepository privacyRepo,
        IRedisService redis)
    {
        _locationRepo = locationRepo;
        _privacyRepo = privacyRepo;
        _redis = redis;
    }

    /// <summary>
    /// Get map card with zones (UI-safe positions)
    /// </summary>
    public async Task<MapCardResponse> GetMapCardAsync(Guid userId)
    {
        // Check user's privacy settings
        var privacy = await _privacyRepo.GetPrivacySettingsAsync(userId);
        if (privacy?.IsLocationEnabled != true)
        {
            return new MapCardResponse
            {
                Zones = new(),
                IsVerifiedOnly = false
            };
        }

        // Get user's location
        var userLocation = await _locationRepo.GetCurrentLocationAsync(userId);
        if (userLocation == null)
        {
            return new MapCardResponse
            {
                Zones = new(),
                IsVerifiedOnly = privacy.VerifiedOnlyMap
            };
        }

        // Try to get from cache first
        var cacheKey = $"map_card:{userId}";
        var cached = await _redis.GetAsync(cacheKey);
        if (cached != null)
        {
            return System.Text.Json.JsonSerializer.Deserialize<MapCardResponse>(cached)
                ?? new MapCardResponse { Zones = new(), IsVerifiedOnly = privacy.VerifiedOnlyMap };
        }

        // Get nearby locations
        var nearbyLocations = await _locationRepo.GetNearbyLocationsAsync(userLocation.Geohash, precision: 5);

        // Group into zones (10x10 grid = 100 zones)
        var zones = new Dictionary<(int, int), int>(); // zone_key -> people_count

        foreach (var location in nearbyLocations)
        {
            // Privacy check
            var otherPrivacy = await _privacyRepo.GetPrivacySettingsAsync(location.UserId);
            if (otherPrivacy?.IsLocationEnabled != true || otherPrivacy.IsPaused)
                continue;

            // Verified-only check
            if (privacy.VerifiedOnlyMap)
            {
                // TODO: Check if user is verified
            }

            // Map geohash to zone position (0-100, 0-100)
            var (x, y) = GeohashToZone(location.Geohash);
            var zoneKey = (x / 10, y / 10); // 10x10 zones

            if (!zones.ContainsKey(zoneKey))
                zones[zoneKey] = 0;

            zones[zoneKey]++;
        }

        // Convert to response
        var response = new MapCardResponse
        {
            Zones = zones.Select(kv => new MapZoneDto
            {
                X = kv.Key.Item1 * 10 + 5, // Center of zone
                Y = kv.Key.Item2 * 10 + 5,
                PeopleCount = kv.Value
            }).ToList(),
            IsVerifiedOnly = privacy.VerifiedOnlyMap
        };

        // Cache for 5 minutes
        await _redis.SetAsync(
            cacheKey,
            System.Text.Json.JsonSerializer.Serialize(response),
            TimeSpan.FromMinutes(5)
        );

        return response;
    }

    /// <summary>
    /// Convert geohash to UI-safe zone position
    /// </summary>
    private (int x, int y) GeohashToZone(string geohash)
    {
        // Simple hash-based mapping (not GPS-based)
        var hash = geohash.GetHashCode();
        var x = Math.Abs(hash % 100);
        var y = Math.Abs((hash / 100) % 100);
        return (x, y);
    }
}
