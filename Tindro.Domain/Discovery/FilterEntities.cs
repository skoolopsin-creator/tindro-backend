namespace Tindro.Domain.Discovery;

using Tindro.Domain.Users;

/// <summary>
/// User's discovery filter preferences
/// </summary>
public class FilterPreferences
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = "Default";
    public bool IsActive { get; set; } = true;

    // Age filter
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;

    // Height filter (in cm)
    public int? MinHeight { get; set; }
    public int? MaxHeight { get; set; }

    // Distance filter (in km)
    public int? MaxDistance { get; set; }

    // Education filter
    public string? EducationLevel { get; set; }

    // Lifestyle preferences
    public string? SmokingPreference { get; set; } // "smoker", "non-smoker", "any"
    public string? DrinkingPreference { get; set; } // "drinker", "non-drinker", "social", "any"
    public string? ExerciseFrequency { get; set; } // "daily", "regularly", "sometimes", "never", "any"

    // Religion
    public string? Religion { get; set; }

    // Relationship goals
    public string? RelationshipGoal { get; set; } // "dating", "long-term", "marriage", "any"

    // Family plans
    public string? FamilyPlans { get; set; } // "wants-kids", "no-kids", "unsure", "any"

    // Personality traits filter
    public bool FilterByPersonality { get; set; }
    public string? PersonalityTraits { get; set; } // comma-separated or serialized list

    // Interests filter
    public bool FilterByInterests { get; set; }
    public int? MinSharedInterests { get; set; } = 2;

    // Verification requirements
    public bool RequireVerified { get; set; }
    public bool RequirePhotos { get; set; } = true;
    public int? MinPhotos { get; set; } = 1;

    // Profile completion
    public int? MinProfileCompletion { get; set; } = 50;

    // Sorting preference
    public string? SortBy { get; set; } = "compatibility"; // "compatibility", "distance", "newest", "verified"

    // Premium filters
    public bool ShowOnlineOnly { get; set; }
    public bool ShowRecentlyActive { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }

    // Navigation
    public virtual User? User { get; set; }
}

/// <summary>
/// Individual filter criteria for advanced filtering
/// </summary>
public class FilterCriteria
{
    public Guid Id { get; set; }
    public Guid FilterPreferencesId { get; set; }
    public string CriteriaType { get; set; } = string.Empty; // "age", "height", "education", etc.
    public string Operator { get; set; } = "="; // "=", "!=", ">", "<", ">=", "<=", "in", "contains"
    public string Value { get; set; } = string.Empty;
    public int Priority { get; set; } // 0-10, higher = more important
    public bool IsRequired { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual FilterPreferences? FilterPreferences { get; set; }
}

/// <summary>
/// Saved filter templates for quick access
/// </summary>
public class SavedFilter
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Guid? FilterPreferencesId { get; set; }
    public bool IsDefault { get; set; }
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastAppliedAt { get; set; }

    // Serialized filter data for quick restoration
    public string FilterData { get; set; } = string.Empty; // JSON serialized FilterPreferences

    // Navigation
    public virtual User? User { get; set; }
    public virtual FilterPreferences? FilterPreferences { get; set; }
}

/// <summary>
/// Filter application history for analytics
/// </summary>
public class FilterApplicationHistory
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid FilterPreferencesId { get; set; }
    public int ResultCount { get; set; }
    public int? ProfilesViewed { get; set; }
    public int? Matches { get; set; }
    public int Messages { get; set; }
    public DateTime AppliedAt { get; set; }
    public DateTime? ExpiresAt { get; set; } // Cache expiry time

    // Navigation
    public virtual User? User { get; set; }
    public virtual FilterPreferences? FilterPreferences { get; set; }
}
