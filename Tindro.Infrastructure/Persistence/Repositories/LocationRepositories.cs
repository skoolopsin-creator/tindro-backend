using Microsoft.EntityFrameworkCore;
using Tindro.Application.Common.Interfaces;
using Tindro.Domain.Location;
using Tindro.Infrastructure.Persistence;

namespace Tindro.Infrastructure.Persistence.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly CommandDbContext _db;

    public LocationRepository(CommandDbContext db)
    {
        _db = db;
    }

    public async Task<UserLocation?> GetCurrentLocationAsync(Guid userId)
    {
        return await _db.Set<UserLocation>()
            .Where(x => x.UserId == userId && x.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();
    }

    public async Task UpdateLocationAsync(Guid userId, string geohash, Guid cityId)
    {
        var existing = await _db.Set<UserLocation>()
            .FirstOrDefaultAsync(x => x.UserId == userId);

        if (existing != null)
        {
            existing.Geohash = geohash;
            existing.CityId = cityId;
            existing.UpdatedAt = DateTime.UtcNow;
            existing.ExpiresAt = DateTime.UtcNow.AddHours(48);
        }
        else
        {
            var location = new UserLocation
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Geohash = geohash,
                CityId = cityId,
                UpdatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(48)
            };

            _db.Set<UserLocation>().Add(location);
        }

        await _db.SaveChangesAsync();
    }

    public async Task<DateTime?> GetLastLocationUpdateAsync(Guid userId)
    {
        return await _db.Set<UserLocation>()
            .Where(x => x.UserId == userId)
            .Select(x => (DateTime?)x.UpdatedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<List<UserLocation>> GetNearbyLocationsAsync(string geohash, int precision = 6)
    {
        // Query users with same geohash (same precision level)
        var truncated = geohash.Substring(0, Math.Min(precision, geohash.Length));

        return await _db.Set<UserLocation>()
            .Where(x => x.Geohash.StartsWith(truncated) && x.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task DeleteExpiredLocationsAsync()
    {
        await _db.Set<UserLocation>()
            .Where(x => x.ExpiresAt <= DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }
}

public class CrossedPathRepository : ICrossedPathRepository
{
    private readonly CommandDbContext _db;

    public CrossedPathRepository(CommandDbContext db)
    {
        _db = db;
    }

    public async Task<bool> ExistsCrossedPathAsync(Guid user1Id, Guid user2Id)
    {
        var orderedId1 = user1Id < user2Id ? user1Id : user2Id;
        var orderedId2 = user1Id < user2Id ? user2Id : user1Id;

        return await _db.Set<CrossedPath>()
            .AnyAsync(x =>
                x.User1Id == orderedId1 &&
                x.User2Id == orderedId2 &&
                x.ExpiresAt > DateTime.UtcNow);
    }

    public async Task CreateCrossedPathAsync(Guid user1Id, Guid user2Id, string geohash)
    {
        // Ensure consistent ordering
        if (user1Id > user2Id)
            (user1Id, user2Id) = (user2Id, user1Id);

        var path = new CrossedPath
        {
            Id = Guid.NewGuid(),
            User1Id = user1Id,
            User2Id = user2Id,
            Geohash = geohash,
            CrossedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        _db.Set<CrossedPath>().Add(path);
        await _db.SaveChangesAsync();
    }

    public async Task<List<CrossedPath>> GetCrossedPathsForUserAsync(Guid userId)
    {
        return await _db.Set<CrossedPath>()
            .Where(x => (x.User1Id == userId || x.User2Id == userId) &&
                        x.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(x => x.CrossedAt)
            .ToListAsync();
    }

    public async Task DeleteExpiredCrossedPathsAsync()
    {
        await _db.Set<CrossedPath>()
            .Where(x => x.ExpiresAt <= DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }
}

public class LocationPrivacyRepository : ILocationPrivacyRepository
{
    private readonly CommandDbContext _db;

    public LocationPrivacyRepository(CommandDbContext db)
    {
        _db = db;
    }

    public async Task<LocationPrivacyPreferences?> GetPrivacySettingsAsync(Guid userId)
    {
        return await _db.Set<LocationPrivacyPreferences>()
            .FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<LocationPrivacyPreferences> GetOrCreatePrivacySettingsAsync(Guid userId)
    {
        var existing = await GetPrivacySettingsAsync(userId);
        if (existing != null)
            return existing;

        var settings = new LocationPrivacyPreferences
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            IsLocationEnabled = false,
            UpdatedAt = DateTime.UtcNow
        };

        _db.Set<LocationPrivacyPreferences>().Add(settings);
        await _db.SaveChangesAsync();

        return settings;
    }

    public async Task UpdatePrivacySettingsAsync(Guid userId, LocationPrivacyPreferences settings)
    {
        var existing = await GetPrivacySettingsAsync(userId);
        if (existing == null)
        {
            settings.UserId = userId;
            _db.Set<LocationPrivacyPreferences>().Add(settings);
        }
        else
        {
            existing.IsLocationEnabled = settings.IsLocationEnabled;
            existing.HideDistance = settings.HideDistance;
            existing.IsPaused = settings.IsPaused;
            existing.VerifiedOnlyMap = settings.VerifiedOnlyMap;
            existing.UpdatedAt = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }
}

public class CityRepository : ICityRepository
{
    private readonly QueryDbContext _db;

    public CityRepository(QueryDbContext db)
    {
        _db = db;
    }

    public async Task<City?> GetCityByIdAsync(Guid cityId)
    {
        return await _db.Set<City>()
            .FirstOrDefaultAsync(x => x.Id == cityId);
    }

    public async Task<City?> FindNearestCityAsync(double latitude, double longitude)
    {
        // PostGIS query would go here
        // For now, simple distance-based fallback
        return await _db.Set<City>()
            .FirstOrDefaultAsync();
    }
}
