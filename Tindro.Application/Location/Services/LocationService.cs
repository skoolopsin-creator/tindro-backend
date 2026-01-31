using Tindro.Application.Location.Dtos;
using Tindro.Application.Common.Interfaces;
using Tindro.Domain.Location;

namespace Tindro.Application.Location.Services;

public class LocationService
{
    private readonly ILocationRepository _locationRepo;
    private readonly ILocationPrivacyRepository _privacyRepo;
    private readonly ICityRepository _cityRepo;
    private readonly GeohashService _geohashService;
    private readonly IRedisService _redis;
    private const int LocationUpdateIntervalMinutes = 30;

    public LocationService(
        ILocationRepository locationRepo,
        ILocationPrivacyRepository privacyRepo,
        ICityRepository cityRepo,
        GeohashService geohashService,
        IRedisService redis)
    {
        _locationRepo = locationRepo;
        _privacyRepo = privacyRepo;
        _cityRepo = cityRepo;
        _geohashService = geohashService;
        _redis = redis;
    }

    /// <summary>
    /// Update user location with privacy measures
    /// </summary>
    public async Task<LocationUpdateResponse> UpdateLocationAsync(Guid userId, LocationUpdateRequest request)
    {
        // Check if user has location enabled
        var privacy = await _privacyRepo.GetOrCreatePrivacySettingsAsync(userId);
        if (!privacy.IsLocationEnabled || privacy.IsPaused)
        {
            return new LocationUpdateResponse
            {
                Success = false,
                Message = "Location sharing is disabled"
            };
        }

        // Check rate limiting (max once every 30 minutes)
        var lastUpdate = await _locationRepo.GetLastLocationUpdateAsync(userId);
        if (lastUpdate.HasValue && DateTime.UtcNow.Subtract(lastUpdate.Value).TotalMinutes < LocationUpdateIntervalMinutes)
        {
            var nextAllowed = lastUpdate.Value.AddMinutes(LocationUpdateIntervalMinutes);
            return new LocationUpdateResponse
            {
                Success = false,
                Message = "Location update too frequent",
                NextUpdateAllowed = nextAllowed
            };
        }

        // Privacy measures:
        // 1. Round coordinates
        var (roundedLat, roundedLng) = _geohashService.RoundCoordinates(request.Latitude, request.Longitude, decimals: 3);

        // 2. Add random noise (+/- 200m)
        var (noisyLat, noisyLng) = _geohashService.AddNoise(roundedLat, roundedLng);

        // 3. Convert to geohash (precision 6 = ~350m)
        var geohash = _geohashService.Encode(noisyLat, noisyLng, precision: 6);

        // 4. Find city
        var city = await _cityRepo.FindNearestCityAsync(noisyLat, noisyLng);
        if (city == null)
        {
            return new LocationUpdateResponse
            {
                Success = false,
                Message = "Could not determine city for this location"
            };
        }

        // 5. Store only geohash & city (no raw GPS)
        await _locationRepo.UpdateLocationAsync(userId, geohash, city.Id);

        // Cache for quick access
        await _redis.SetAsync($"user_location:{userId}", geohash, TimeSpan.FromHours(1));

        return new LocationUpdateResponse
        {
            Success = true,
            Message = "Location updated successfully"
        };
    }

    /// <summary>
    /// Get nearby users with privacy filters
    /// </summary>
    public async Task<NearbyUsersResponse> GetNearbyUsersAsync(
        Guid userId,
        double radiusKm = 5,
        int? ageMin = null,
        int? ageMax = null,
        string? genderPreference = null)
    {
        var currentLocation = await _locationRepo.GetCurrentLocationAsync(userId);
        if (currentLocation == null)
        {
            return new NearbyUsersResponse { Users = new(), TotalCount = 0 };
        }

        // Get nearby geohashes with expanded search
        var nearbyLocations = await _locationRepo.GetNearbyLocationsAsync(currentLocation.Geohash, precision: 6);

        // Calculate actual distances using Haversine
        var response = new NearbyUsersResponse { Users = new() };

        foreach (var location in nearbyLocations.Where(l => l.UserId != userId))
        {
            // Apply privacy filters
            var privacy = await _privacyRepo.GetPrivacySettingsAsync(location.UserId);
            if (privacy?.IsPaused == true)
                continue;

            // Fuzzy distance (never exact GPS)
            var distance = FuzzyDistance(radiusKm);

            var user = new NearbyUserDto
            {
                UserId = location.UserId,
                Username = $"User{location.UserId.ToString().Substring(0, 8)}",
                Age = 25, // TODO: Get from profile
                ProfilePhoto = "", // TODO: Get from profile
                Distance = distance,
                IsVerified = false // TODO: Get from profile
            };

            response.Users.Add(user);
        }

        response.TotalCount = response.Users.Count;
        return response;
    }

    /// <summary>
    /// Get fuzzy distance for privacy
    /// </summary>
    private string FuzzyDistance(double actualKm)
    {
        return actualKm switch
        {
            < 0.5 => "Near you",
            < 2 => "1 km",
            < 5 => "3 km",
            < 10 => "5 km",
            _ => $"{Math.Round(actualKm)} km"
        };
    }

    /// <summary>
    /// Haversine formula for distance calculation
    /// </summary>
    public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in km
        var dLat = (lat2 - lat1) * Math.PI / 180;
        var dLon = (lon2 - lon1) * Math.PI / 180;
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
}
