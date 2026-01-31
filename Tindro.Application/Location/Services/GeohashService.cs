namespace Tindro.Application.Location.Services;

/// <summary>
/// Geohashing utility for location privacy
/// Converts GPS coordinates to geohash with configurable precision
/// </summary>
public class GeohashService
{
    private const string Base32 = "0123456789bcdefghjkmnpqrstuvwxyz";
    private readonly Random _random = new();

    /// <summary>
    /// Add random noise to coordinates (+/- 200m)
    /// </summary>
    public (double latitude, double longitude) AddNoise(double latitude, double longitude)
    {
        // 1 degree ≈ 111 km, so 0.002 degrees ≈ 222 meters
        var noiseRange = 0.002;
        var noiseLat = (_random.NextDouble() - 0.5) * 2 * noiseRange;
        var noiseLng = (_random.NextDouble() - 0.5) * 2 * noiseRange;

        return (
            latitude + noiseLat,
            longitude + noiseLng
        );
    }

    /// <summary>
    /// Round coordinates to reduce precision
    /// </summary>
    public (double latitude, double longitude) RoundCoordinates(double latitude, double longitude, int decimals = 3)
    {
        return (
            Math.Round(latitude, decimals),
            Math.Round(longitude, decimals)
        );
    }

    /// <summary>
    /// Encode latitude/longitude to geohash
    /// Precision levels:
    /// - 4: ~5.6 km
    /// - 5: ~1.4 km
    /// - 6: ~0.35 km (350m)
    /// - 7: ~87m
    /// </summary>
    public string Encode(double latitude, double longitude, int precision = 6)
    {
        var geohash = "";
        var isEven = true;
        double latMin = -90, latMax = 90, lngMin = -180, lngMax = 180;

        for (int i = 0; i < precision; i++)
        {
            int bit = 0;

            if (isEven)
            {
                var mid = (lngMin + lngMax) / 2;
                if (longitude >= mid)
                {
                    bit |= 1;
                    lngMin = mid;
                }
                else
                {
                    lngMax = mid;
                }
            }
            else
            {
                var mid = (latMin + latMax) / 2;
                if (latitude >= mid)
                {
                    bit |= 1;
                    latMin = mid;
                }
                else
                {
                    latMax = mid;
                }
            }

            isEven = !isEven;

            if ((i + 1) % 5 == 0)
            {
                geohash += Base32[bit];
                bit = 0;
            }
            else
            {
                bit <<= 1;
            }
        }

        return geohash;
    }

    /// <summary>
    /// Get neighbors of a geohash (for expanded search)
    /// </summary>
    public List<string> GetNeighbors(string geohash)
    {
        var neighbors = new List<string>();
        var right = ShiftGeohash(geohash, 1, 0);
        var left = ShiftGeohash(geohash, -1, 0);
        var top = ShiftGeohash(geohash, 0, 1);
        var bottom = ShiftGeohash(geohash, 0, -1);

        neighbors.Add(geohash);
        neighbors.Add(right);
        neighbors.Add(left);
        neighbors.Add(top);
        neighbors.Add(bottom);

        return neighbors;
    }

    private string ShiftGeohash(string geohash, int lngShift, int latShift)
    {
        // Simplified neighbor calculation
        // In production, use a proper geohash library like GeoHash.Net
        return geohash; // Placeholder
    }
}
