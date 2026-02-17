using Tindro.Domain.Common;
using Tindro.Domain.Users;

namespace Tindro.Domain.Recommendations;

public class UserPreferences
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    // Age preferences
    public int MinAgePreference { get; set; } = 18;
    public int MaxAgePreference { get; set; } = 50;

    // Height preferences (in cm)
    public int? MinHeightPreference { get; set; }
    public int? MaxHeightPreference { get; set; }

    // Distance preference (in km)
    public int MaxDistancePreference { get; set; } = 50;

    // Lifestyle preferences
    public bool? SmokingPreference { get; set; } // null = no preference
    public bool? DrinkingPreference { get; set; }
    public bool? WantChildrenPreference { get; set; }
    public bool? HaveChildrenPreference { get; set; }

    // Education preference
    public string? EducationPreference { get; set; } // "HighSchool", "Bachelor", "Master", "PhD"

    // Relationship type
    public string? RelationshipType { get; set; } // "Dating", "Casual", "LongTerm", "Friends"

    // Religion/Ethnicity preferences
    public string? ReligionPreference { get; set; }
    public string? EthnicityPreference { get; set; }

    // Interests (comma-separated or list)
    public List<string> InterestCategories { get; set; } = new();

    // Personality traits preference
    public List<string> PersonalityTraits { get; set; } = new();

    // Filtering settings
    public bool OnlyVerifiedProfiles { get; set; } = false;
    public bool HideInactiveProfiles { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

public class RecommendationScore
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RecommendedUserId { get; set; }

    // Scoring components (0-100)
    public decimal AgeCompatibility { get; set; }
    public decimal LocationCompatibility { get; set; }
    public decimal InterestCompatibility { get; set; }
    public decimal LifestyleCompatibility { get; set; }
    public decimal ProfileCompleteness { get; set; }
    public decimal VerificationScore { get; set; }

    // Overall score (weighted average)
    public decimal OverallScore { get; set; }

    // Timestamp for cache invalidation
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddHours(24);

    // Track if user has interacted
    public bool? HasLiked { get; set; }
    public bool? HasSkipped { get; set; }
    public bool? HasMatched { get; set; }
}

public class UserInterest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid InterestId { get; set; }


    public int ConfidenceScore { get; set; } = 50;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; }
    public Interest Interest { get; set; }
}

public class SkipProfile
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid SkippedUserId { get; set; }

    public string? Reason { get; set; } // "NotInterested", "LookingForSomethingElse", "NotMyType", etc.
    
    public DateTime SkippedAt { get; set; } = DateTime.UtcNow;

    // TTL for reintroduction
    public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(90);
}

public class RecommendationFeedback
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RecommendedUserId { get; set; }

    public string FeedbackType { get; set; } = string.Empty; // "Like", "Skip", "Report", "Block"
    public string? Reason { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
