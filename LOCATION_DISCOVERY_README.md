# Location-Based Discovery System

## Overview

A privacy-first location-based discovery system with geohashing, crossed paths detection, and UI-safe map visualization. No raw GPS coordinates are ever stored or transmitted.

## Features

✅ **Near Me Discovery** - Find nearby users within configurable radius  
✅ **Crossed Paths** - Discover users you've encountered  
✅ **Location Privacy** - Opt-in, with fuzzy distances and pausing  
✅ **Geohashing** - GPS coordinates converted to geohashes (+/- 200m noise)  
✅ **Map Cards** - UI-safe zone visualization (no GPS)  
✅ **Data Retention** - Auto-delete after 48h (locations) / 7d (crossed paths)  
✅ **Background Jobs** - Automatic cleanup and path discovery  

---

## Architecture

### Database Schema

```sql
-- User Locations (48h TTL)
CREATE TABLE user_locations (
  id UUID PRIMARY KEY,
  user_id UUID UNIQUE NOT NULL,
  geohash VARCHAR(12) NOT NULL,
  city_id UUID NOT NULL REFERENCES cities(id),
  updated_at TIMESTAMP NOT NULL,
  expires_at TIMESTAMP NOT NULL,
  CHECK (expires_at > updated_at)
);
CREATE INDEX idx_user_locations_geohash ON user_locations(geohash);
CREATE INDEX idx_user_locations_expires_at ON user_locations(expires_at);

-- Crossed Paths (7d TTL)
CREATE TABLE crossed_paths (
  id UUID PRIMARY KEY,
  user1_id UUID NOT NULL,
  user2_id UUID NOT NULL,
  geohash VARCHAR(12) NOT NULL,
  crossed_at TIMESTAMP NOT NULL,
  expires_at TIMESTAMP NOT NULL,
  UNIQUE(user1_id, user2_id),
  CHECK (user1_id < user2_id)
);
CREATE INDEX idx_crossed_paths_expires_at ON crossed_paths(expires_at);

-- Privacy Preferences
CREATE TABLE location_privacy_preferences (
  id UUID PRIMARY KEY,
  user_id UUID UNIQUE NOT NULL,
  is_location_enabled BOOLEAN NOT NULL DEFAULT FALSE,
  hide_distance BOOLEAN NOT NULL DEFAULT FALSE,
  is_paused BOOLEAN NOT NULL DEFAULT FALSE,
  verified_only_map BOOLEAN NOT NULL DEFAULT FALSE,
  updated_at TIMESTAMP NOT NULL
);

-- Cities
CREATE TABLE cities (
  id UUID PRIMARY KEY,
  name VARCHAR(255) NOT NULL,
  country VARCHAR(100) NOT NULL,
  location GEOMETRY(POINT, 4326) NOT NULL,
  UNIQUE(name, country)
);
```

### Privacy Layers

1. **GPS Coordinates**
   - Never stored
   - Rounded to 3 decimals
   - Noise added (+/- 200m)

2. **Geohash**
   - Precision 6 = ~350m accuracy
   - Used for nearby queries
   - Never transmitted to client

3. **Fuzzy Distances**
   - "Near you" (< 0.5 km)
   - "1 km", "3 km", "5 km"
   - Randomized display

4. **Zone-Based Map**
   - 10x10 grid (100 zones)
   - Aggregated user counts
   - No GPS coordinates

---

## API Endpoints

### Location Update
```http
POST /api/v1/location/update
Authorization: Bearer {token}
Content-Type: application/json

{
  "latitude": 40.7128,
  "longitude": -74.0060
}

Response 200:
{
  "success": true,
  "message": "Location updated successfully",
  "nextUpdateAllowed": null
}

Response 429:
{
  "success": false,
  "message": "Location update too frequent",
  "nextUpdateAllowed": "2024-01-27T15:45:00Z"
}
```

### Nearby Users
```http
GET /api/v1/location/nearby?radius=5&ageMin=20&ageMax=30
Authorization: Bearer {token}

Response 200:
{
  "users": [
    {
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "username": "User550e84",
      "age": 25,
      "profilePhoto": "https://...",
      "distance": "2 km",
      "isVerified": true
    }
  ],
  "totalCount": 1
}
```

### Crossed Paths
```http
GET /api/v1/discovery/crossed-paths
Authorization: Bearer {token}

Response 200:
{
  "paths": [
    {
      "userId": "550e8400-e29b-41d4-a716-446655440000",
      "username": "User550e84",
      "profilePhoto": "https://...",
      "age": 25,
      "crossedAt": "Today",
      "location": "New York"
    }
  ],
  "totalCount": 1
}
```

### Map Card
```http
GET /api/v1/discovery/map-card
Authorization: Bearer {token}

Response 200:
{
  "zones": [
    {
      "x": 25,
      "y": 45,
      "peopleCount": 3
    },
    {
      "x": 55,
      "y": 75,
      "peopleCount": 1
    }
  ],
  "isVerifiedOnly": false
}
```

### Privacy Settings
```http
GET /api/v1/discovery/privacy-settings
Authorization: Bearer {token}

Response 200:
{
  "isLocationEnabled": true,
  "hideDistance": false,
  "isPaused": false,
  "verifiedOnlyMap": false
}

---

POST /api/v1/discovery/privacy-settings
Authorization: Bearer {token}
Content-Type: application/json

{
  "isLocationEnabled": true,
  "hideDistance": false,
  "isPaused": false,
  "verifiedOnlyMap": true
}

Response 200:
{
  "isLocationEnabled": true,
  "hideDistance": false,
  "isPaused": false,
  "verifiedOnlyMap": true
}
```

---

## Service Layer

### LocationService
Handles location updates with privacy measures
- `UpdateLocationAsync()` - Coordinates → Geohash + City
- `GetNearbyUsersAsync()` - Query nearby users with filters

### CrossedPathsService
Detects and manages crossed paths
- `FindCrossedPathsAsync()` - Background job trigger
- `GetCrossedPathsAsync()` - User crossed path history

### MapCardService
Generates UI-safe map visualizations
- `GetMapCardAsync()` - Zone-based aggregation

### GeohashService
Converts GPS to geohash with privacy
- `AddNoise()` - Adds random ±200m noise
- `RoundCoordinates()` - Reduces precision
- `Encode()` - Converts to geohash
- `GetNeighbors()` - Expanded search radius

---

## Background Jobs

### CrossedPathsJob
Triggered after each location update
- Finds users at same geohash within 30min window
- Creates crossed_paths records
- TTL: 7 days

### LocationRetentionJob
Scheduled: Daily at 2 AM
- Deletes locations older than 48 hours
- Automatic via ExpiresAt column

### CrossedPathsRetentionJob
Scheduled: Daily at 3 AM
- Deletes crossed paths older than 7 days
- Automatic via ExpiresAt column

---

## Privacy & Safety

### Rate Limiting
- Location updates: Max 1 per 30 minutes
- Enforced per-user in LocationService

### Opt-In Model
- Location disabled by default
- User must explicitly enable
- Can pause at any time

### Verified-Only Map
- Optional setting for enhanced privacy
- Only shows locations to verified users
- Useful for safety-conscious users

### Data Minimization
- No raw GPS stored
- Geohash precision: ~350m
- Coordinates rounded & noised
- Distances randomized/fuzzy
- Timestamps fuzzy ("Today", "This week")

### Auto-Deletion
- User locations: 48 hours
- Crossed paths: 7 days
- No long-term tracking

---

## Setup Instructions

### 1. Database Setup

```bash
# Enable PostGIS extension
psql -U postgres -d tindro_command
CREATE EXTENSION IF NOT EXISTS postgis;
```

### 2. Run Migrations

```bash
dotnet ef migrations add AddLocationDiscovery -p Tindro.Infrastructure -s Tindro.Api
dotnet ef database update
```

### 3. Register Services

Services are auto-registered in `DependencyInjection.cs`:
```csharp
services.AddScoped<ILocationRepository, LocationRepository>();
services.AddScoped<ICrossedPathRepository, CrossedPathRepository>();
services.AddScoped<ILocationPrivacyRepository, LocationPrivacyRepository>();
services.AddScoped<ICityRepository, CityRepository>();
services.AddScoped<GeohashService>();
services.AddScoped<LocationService>();
services.AddScoped<CrossedPathsService>();
services.AddScoped<MapCardService>();
```

### 4. Background Jobs (Hangfire)

```csharp
// In Program.cs
RecurringJob.AddOrUpdate<LocationRetentionJob>(
    "location-retention",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(2, 0) // 2 AM daily
);

RecurringJob.AddOrUpdate<CrossedPathsRetentionJob>(
    "crossed-paths-retention",
    job => job.ExecuteAsync(CancellationToken.None),
    Cron.Daily(3, 0) // 3 AM daily
);
```

---

## Testing

### Manual Testing

```bash
# 1. Enable location for user
curl -X POST https://api.example.com/api/v1/discovery/privacy-settings \
  -H "Authorization: Bearer {token}" \
  -d '{"isLocationEnabled": true}'

# 2. Update location
curl -X POST https://api.example.com/api/v1/location/update \
  -H "Authorization: Bearer {token}" \
  -d '{"latitude": 40.7128, "longitude": -74.0060}'

# 3. Get nearby users
curl https://api.example.com/api/v1/location/nearby?radius=5 \
  -H "Authorization: Bearer {token}"

# 4. Get map card
curl https://api.example.com/api/v1/discovery/map-card \
  -H "Authorization: Bearer {token}"

# 5. Get crossed paths
curl https://api.example.com/api/v1/discovery/crossed-paths \
  -H "Authorization: Bearer {token}"
```

### Load Testing

```csharp
// Simulate 1000 users
for (int i = 0; i < 1000; i++)
{
    var lat = 40.7128 + (i * 0.001);
    var lon = -74.0060 + (i * 0.001);
    await locationService.UpdateLocationAsync(userId, new LocationUpdateRequest
    {
        Latitude = lat,
        Longitude = lon
    });
}
```

---

## Performance Considerations

### Geohashing
- Precision 6: ~350m, good for discovery
- Precision 7: ~87m, better accuracy
- Neighbors: Expanded search to 9 adjacent cells

### Database Indexes
- Index on Geohash for fast nearby queries
- Index on ExpiresAt for cleanup jobs
- Unique index on User1Id, User2Id for crossed paths

### Caching
- Map cards cached 5 minutes
- Location validity cached 1 hour
- Redis for quick lookups

### Query Optimization
- Geohash prefix matching (substring)
- Batch deletions via ExecuteDeleteAsync
- Minimal joins (geohash-based)

---

## Security Checklist

- ✅ No raw GPS stored
- ✅ Geohash with noise
- ✅ Fuzzy distances
- ✅ Opt-in required
- ✅ Can pause anytime
- ✅ Auto-deletion
- ✅ User ordering (User1Id < User2Id)
- ✅ Authorization on all endpoints
- ✅ Rate limiting per endpoint

---

## Future Enhancements

1. **PostGIS Integration** - Proper ST_DWithin queries
2. **Geofencing** - Notify on entry/exit of zones
3. **Real-Time Updates** - Push notifications for nearby arrivals
4. **Privacy Audit Log** - Track location sharing
5. **Advanced Filtering** - Persona, interests, verification
6. **Analytics** - Heatmaps, popular zones

---

## Troubleshooting

### Location updates too frequent
- Error: "Location update too frequent"
- Reason: Updates limited to 1 per 30 minutes
- Solution: Wait for nextUpdateAllowed timestamp

### No nearby users found
- Check user location is enabled
- Verify geohash precision level
- Ensure other users are within radius + buffer

### Crossed paths not appearing
- Require simultaneous geohash presence
- 30-minute window enforced
- Both users must have location enabled

### Map card shows no zones
- User's location must be enabled
- Other users in same city required
- Check privacy settings of nearby users

---

## API Reference

| Endpoint | Method | Auth | Rate Limit |
|----------|--------|------|-----------|
| /api/v1/location/update | POST | Required | 1 per 30 min |
| /api/v1/location/nearby | GET | Required | 100 per hour |
| /api/v1/discovery/crossed-paths | GET | Required | 100 per hour |
| /api/v1/discovery/map-card | GET | Required | 200 per hour |
| /api/v1/discovery/privacy-settings | GET/POST | Required | 100 per hour |

---

**Version**: 1.0  
**Last Updated**: January 27, 2026  
**Status**: Production Ready
