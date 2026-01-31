namespace Tindro.Application.Discovery.Dtos;

/// <summary>
/// Filter preferences request/response DTO
/// </summary>
public class FilterPreferencesDto
{
    public Guid? Id { get; set; }
    public string Name { get; set; } = "Default";
    public bool IsActive { get; set; } = true;

    // Age filter
    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;

    // Height filter (cm)
    public int? MinHeight { get; set; }
    public int? MaxHeight { get; set; }

    // Distance filter (km)
    public int? MaxDistance { get; set; }

    // Education
    public string? EducationLevel { get; set; }

    // Lifestyle
    public string? SmokingPreference { get; set; }
    public string? DrinkingPreference { get; set; }
    public string? ExerciseFrequency { get; set; }

    // Religion
    public string? Religion { get; set; }

    // Relationship goals
    public string? RelationshipGoal { get; set; }

    // Family plans
    public string? FamilyPlans { get; set; }

    // Personality
    public bool FilterByPersonality { get; set; }
    public string? PersonalityTraits { get; set; }

    // Interests
    public bool FilterByInterests { get; set; }
    public int? MinSharedInterests { get; set; } = 2;

    // Verification requirements
    public bool RequireVerified { get; set; }
    public bool RequirePhotos { get; set; } = true;
    public int? MinPhotos { get; set; } = 1;

    // Profile completion
    public int? MinProfileCompletion { get; set; } = 50;

    // Sorting
    public string? SortBy { get; set; } = "compatibility";

    // Premium filters
    public bool ShowOnlineOnly { get; set; }
    public bool ShowRecentlyActive { get; set; }
}

/// <summary>
/// Individual filter criteria DTO
/// </summary>
public class FilterCriteriaDto
{
    public Guid? Id { get; set; }
    public string CriteriaType { get; set; } = string.Empty;
    public string Operator { get; set; } = "=";
    public string Value { get; set; } = string.Empty;
    public int Priority { get; set; } = 5;
    public bool IsRequired { get; set; }
}

/// <summary>
/// Saved filter template DTO
/// </summary>
public class SavedFilterDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastAppliedAt { get; set; }
    public FilterPreferencesDto? FilterPreferences { get; set; }
}

/// <summary>
/// Request to apply discovery filters
/// </summary>
public class ApplyFilterRequestDto
{
    public Guid? SavedFilterId { get; set; }
    public FilterPreferencesDto? FilterPreferences { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

/// <summary>
/// Filter application result
/// </summary>
public class FilterResultDto
{
    public List<UserProfileDto> Profiles { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int CriteriaMatched { get; set; }
    public string? AppliedFilterName { get; set; }
    public DateTime AppliedAt { get; set; }
}

/// <summary>
/// User profile summary for filter results
/// </summary>
public class UserProfileDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public int Age { get; set; }
    public int? Height { get; set; }
    public string? EducationLevel { get; set; }
    public string? Occupation { get; set; }
    public string? Religion { get; set; }
    public decimal? Distance { get; set; }
    public List<string> PhotoUrls { get; set; } = new();
    public List<string> Interests { get; set; } = new();
    public string? Bio { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOnline { get; set; }
    public DateTime? LastSeen { get; set; }
    public int ProfileCompletion { get; set; }
    public List<BadgeDto> Badges { get; set; } = new();
    public decimal? CompatibilityScore { get; set; }
    public int MatchPercentage { get; set; }
}

/// <summary>
/// Badge information
/// </summary>
public class BadgeDto
{
    public Guid Id { get; set; }
    public string BadgeType { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string BadgeIcon { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime AwardedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Filter analytics DTO
/// </summary>
public class FilterAnalyticsDto
{
    public int TotalFiltersUsed { get; set; }
    public int MostFrequentFilter { get; set; }
    public int AverageResultsPerFilter { get; set; }
    public int ConversionRate { get; set; } // Percentage of profiles viewed
    public int MatchRate { get; set; }
    public List<FilterUsageDto> TopFilters { get; set; } = new();
}

/// <summary>
/// Individual filter usage statistics
/// </summary>
public class FilterUsageDto
{
    public string FilterName { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public int ResultCount { get; set; }
    public int ProfilesViewed { get; set; }
    public int Matches { get; set; }
    public decimal ConversionPercentage { get; set; }
    public DateTime LastUsed { get; set; }
}

/// <summary>
/// Advanced filter validation result
/// </summary>
public class FilterValidationResultDto
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public int? EstimatedResultCount { get; set; }
    public string? SuggestedOptimization { get; set; }
}
