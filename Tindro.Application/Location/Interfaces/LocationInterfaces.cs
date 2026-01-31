using Tindro.Domain.Location;

namespace Tindro.Application.Common.Interfaces;

public interface ILocationRepository
{
    Task<UserLocation?> GetCurrentLocationAsync(Guid userId);
    Task UpdateLocationAsync(Guid userId, string geohash, Guid cityId);
    Task<DateTime?> GetLastLocationUpdateAsync(Guid userId);
    Task<List<UserLocation>> GetNearbyLocationsAsync(string geohash, int precision = 6);
    Task DeleteExpiredLocationsAsync();
}

public interface ICrossedPathRepository
{
    Task<bool> ExistsCrossedPathAsync(Guid user1Id, Guid user2Id);
    Task CreateCrossedPathAsync(Guid user1Id, Guid user2Id, string geohash);
    Task<List<CrossedPath>> GetCrossedPathsForUserAsync(Guid userId);
    Task DeleteExpiredCrossedPathsAsync();
}

public interface ILocationPrivacyRepository
{
    Task<LocationPrivacyPreferences?> GetPrivacySettingsAsync(Guid userId);
    Task<LocationPrivacyPreferences> GetOrCreatePrivacySettingsAsync(Guid userId);
    Task UpdatePrivacySettingsAsync(Guid userId, LocationPrivacyPreferences settings);
}

public interface ICityRepository
{
    Task<City?> GetCityByIdAsync(Guid cityId);
    Task<City?> FindNearestCityAsync(double latitude, double longitude);
}
