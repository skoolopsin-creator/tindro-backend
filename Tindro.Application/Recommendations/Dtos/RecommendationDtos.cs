namespace Tindro.Application.Recommendations.Dtos;

public class UserPreferencesDto
{
    public int MinAgePreference { get; set; } = 18;
    public int MaxAgePreference { get; set; } = 50;
    public int? MinHeightPreference { get; set; }
    public int? MaxHeightPreference { get; set; }
    public int MaxDistancePreference { get; set; } = 50;
    public bool? SmokingPreference { get; set; }
    public bool? DrinkingPreference { get; set; }
    public bool? WantChildrenPreference { get; set; }
    public bool? HaveChildrenPreference { get; set; }
    public string? EducationPreference { get; set; }
    public string? RelationshipType { get; set; }
    public string? ReligionPreference { get; set; }
    public string? EthnicityPreference { get; set; }
    public List<string> InterestCategories { get; set; } = new();
    public List<string> PersonalityTraits { get; set; } = new();
    public bool OnlyVerifiedProfiles { get; set; } = false;
    public bool HideInactiveProfiles { get; set; } = true;
}

public class RecommendationDto
{
    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public string? FirstName { get; set; }
    public string? PhotoUrl { get; set; }
    public int Age { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public List<string> Interests { get; set; } = new();
    public decimal CompatibilityScore { get; set; }
    public string? CompatibilityBreakdown { get; set; }
    public bool IsVerified { get; set; }
    public bool IsOnline { get; set; }
}

public class CompatibilityScoreDto
{
    public Guid UserId { get; set; }
    public decimal AgeCompatibility { get; set; }
    public decimal LocationCompatibility { get; set; }
    public decimal InterestCompatibility { get; set; }
    public decimal LifestyleCompatibility { get; set; }
    public decimal ProfileCompleteness { get; set; }
    public decimal VerificationScore { get; set; }
    public decimal OverallScore { get; set; }
    public string[] TopReasons { get; set; } = Array.Empty<string>();
}

public class RecommendationResultDto
{
    public List<RecommendationDto> Recommendations { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public bool HasMore { get; set; }
}

public class UserInterestDto
{
    public Guid InterestId { get; set; }
    public string InterestName { get; set; }
    public string Category { get; set; }
    public string IconKey { get; set; }
    public int ConfidenceScore { get; set; }
}


public class RecommendationFilterDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public int? MaxDistance { get; set; }
    public string? SortBy { get; set; } = "score"; 
    public List<string>? InterestFilter { get; set; }
}

public class AddInterestRequest
{
    public Guid InterestId { get; set; }
    public int ConfidenceScore { get; set; } = 50;
}

public class SkipProfileRequest
{
    public Guid SkippedUserId { get; set; }
    public string? Reason { get; set; }
}

public class ProfileScoreBreakdownDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string? Reason { get; set; }
}
