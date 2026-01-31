namespace Tindro.Application.Location.Dtos;

public class LocationUpdateRequest
{
    /// <summary>
    /// Latitude (-90 to 90)
    /// </summary>
    public double Latitude { get; set; }
    
    /// <summary>
    /// Longitude (-180 to 180)
    /// </summary>
    public double Longitude { get; set; }
}

public class LocationUpdateResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public DateTime? NextUpdateAllowed { get; set; }
}

public class NearbyUserDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public int Age { get; set; }
    public string ProfilePhoto { get; set; } = null!;
    
    /// <summary>
    /// Fuzzy distance: "2 km", "5 km", "Near you"
    /// </summary>
    public string Distance { get; set; } = null!;
    
    /// <summary>
    /// Verification badge
    /// </summary>
    public bool IsVerified { get; set; }
}

public class NearbyUsersResponse
{
    public List<NearbyUserDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
}

public class CrossedPathDto
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string ProfilePhoto { get; set; } = null!;
    public int Age { get; set; }
    
    /// <summary>
    /// When you crossed paths (fuzzy)
    /// </summary>
    public string CrossedAt { get; set; } = null!;
    
    /// <summary>
    /// Where you crossed paths (city name)
    /// </summary>
    public string Location { get; set; } = null!;
}

public class CrossedPathsResponse
{
    public List<CrossedPathDto> Paths { get; set; } = new();
    public int TotalCount { get; set; }
}

public class MapZoneDto
{
    /// <summary>
    /// X position (0-100) on map
    /// </summary>
    public int X { get; set; }
    
    /// <summary>
    /// Y position (0-100) on map
    /// </summary>
    public int Y { get; set; }
    
    /// <summary>
    /// Number of people in this zone
    /// </summary>
    public int PeopleCount { get; set; }
}

public class MapCardResponse
{
    public List<MapZoneDto> Zones { get; set; } = new();
    public bool IsVerifiedOnly { get; set; }
}

public class LocationPrivacySettingsRequest
{
    public bool IsLocationEnabled { get; set; }
    public bool HideDistance { get; set; }
    public bool IsPaused { get; set; }
    public bool VerifiedOnlyMap { get; set; }
}

public class LocationPrivacySettingsResponse
{
    public bool IsLocationEnabled { get; set; }
    public bool HideDistance { get; set; }
    public bool IsPaused { get; set; }
    public bool VerifiedOnlyMap { get; set; }
}
