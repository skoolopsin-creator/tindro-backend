namespace Tindro.Domain.Location;

public class UserLocation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    /// <summary>
    /// Geohash for location (no raw GPS stored)
    /// </summary>
    public string Geohash { get; set; } = null!;
    
    /// <summary>
    /// City ID for fallback discovery
    /// </summary>
    public Guid CityId { get; set; }
    
    /// <summary>
    /// Last update timestamp
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Expiry time (48 hours from update)
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

public class CrossedPath
{
    public Guid Id { get; set; }
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    
    /// <summary>
    /// Geohash where they crossed
    /// </summary>
    public string Geohash { get; set; } = null!;
    
    /// <summary>
    /// When they were at same location
    /// </summary>
    public DateTime CrossedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Expiry time (7 days from creation)
    /// </summary>
    public DateTime ExpiresAt { get; set; }
}

public class LocationPrivacyPreferences
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    
    /// <summary>
    /// User has opted into location-based discovery
    /// </summary>
    public bool IsLocationEnabled { get; set; } = false;
    
    /// <summary>
    /// Hide exact distance from profiles
    /// </summary>
    public bool HideDistance { get; set; } = false;
    
    /// <summary>
    /// Temporarily pause location sharing
    /// </summary>
    public bool IsPaused { get; set; } = false;
    
    /// <summary>
    /// Only show to verified users
    /// </summary>
    public bool VerifiedOnlyMap { get; set; } = false;
    
    /// <summary>
    /// Last updated
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class City
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Country { get; set; } = null!;
    
    /// <summary>
    /// PostGIS Point geometry
    /// </summary>
    public string Location { get; set; } = null!; // WKT format: POINT(lon lat)
}
