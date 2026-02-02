using Tindro.Domain.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tindro.Domain.Users;

public class User : AuditableEntity
{
    public Guid Id { get; set; }
    public string FirebaseUid { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public bool IsVerified { get; set; }

    public Profile Profile { get; set; } = null!;

    public bool IsShadowBanned { get; set; }

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    // Popularity scoring helpers (stubs for background job calculations)
    public double PopularityScore { get; private set; }

    public int LikeCountLast30Days => 0;
    public int MatchCountLast30Days => 0;
    public int MessageCountSentLast30Days => 0;
    public int PhotoCount => Profile?.Photos?.Count ?? 0;
    public bool HasBio => !string.IsNullOrWhiteSpace(Profile?.Bio);
    public bool HasActiveSubscription => false;

    public int Age => Profile == null ? 0 :
        (int)(DateTime.UtcNow.Subtract(Profile.DateOfBirth).TotalDays / 365.25);

    public void UpdatePopularityScore(double score)
    {
        PopularityScore = score;
    }


}
