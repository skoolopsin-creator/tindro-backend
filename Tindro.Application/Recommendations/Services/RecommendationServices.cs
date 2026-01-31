using Tindro.Application.Recommendations.Interfaces;
using Tindro.Application.Recommendations.Dtos;
using Tindro.Domain.Recommendations;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Recommendations.Services;

public class RecommendationService : IRecommendationService
{
    private readonly IRecommendationRepository _recommendationRepo;
    private readonly IPreferenceRepository _preferenceRepo;
    private readonly ISkipRepository _skipRepo;
    private readonly IPreferenceMatchingService _preferenceMatching;
    private readonly IInterestMatchingService _interestMatching;
    private readonly IProfileScoreService _profileScore;

    public RecommendationService(
        IRecommendationRepository recommendationRepo,
        IPreferenceRepository preferenceRepo,
        ISkipRepository skipRepo,
        IPreferenceMatchingService preferenceMatching,
        IInterestMatchingService interestMatching,
        IProfileScoreService profileScore)
    {
        _recommendationRepo = recommendationRepo;
        _preferenceRepo = preferenceRepo;
        _skipRepo = skipRepo;
        _preferenceMatching = preferenceMatching;
        _interestMatching = interestMatching;
        _profileScore = profileScore;
    }

    public async Task<IEnumerable<RecommendationDto>> GetRecommendationsAsync(Guid userId, RecommendationFilterDto filter)
    {
        var recommendations = await _recommendationRepo.GetRecommendationsAsync(userId, filter.Page, filter.PageSize);
        var result = new List<RecommendationDto>();

        foreach (var rec in recommendations.Where(r => r.OverallScore > 30)) // Only scores > 30
        {
            var dto = new RecommendationDto
            {
                UserId = rec.RecommendedUserId,
                CompatibilityScore = rec.OverallScore,
                CompatibilityBreakdown = FormatBreakdown(rec)
            };
            result.Add(dto);
        }

        return result;
    }

    public async Task<CompatibilityScoreDto> CalculateCompatibilityAsync(Guid userId, Guid targetUserId)
    {
        var ageCompat = 0m; // Calculate from profiles
        var locationCompat =  _preferenceMatching.CalculateLocationCompatibility("", new UserPreferences());
        var interestCompat =  _interestMatching.CalculateInterestCompatibility(userId, targetUserId);
        var lifestyleCompat = 0m; // Calculate from preferences
        var profileCompleteness = _profileScore.CalculateProfileCompletenessScore(targetUserId);
        var verificationScore = _profileScore.CalculateVerificationScore(targetUserId);

        var compatibility = new CompatibilityScoreDto
        {
            UserId = targetUserId,
            AgeCompatibility = ageCompat,
            LocationCompatibility = locationCompat,
            InterestCompatibility = interestCompat,
            LifestyleCompatibility = lifestyleCompat,
            ProfileCompleteness = profileCompleteness,
            VerificationScore = verificationScore,
            OverallScore = _profileScore.CalculateOverallScore(new CompatibilityScoreDto())
        };

        return compatibility;
    }

    public async Task<RecommendationScore> SaveRecommendationScoreAsync(Guid userId, Guid targetUserId)
    {
        var existing = await _recommendationRepo.GetRecommendationScoreAsync(userId, targetUserId);
        var compatibility = await CalculateCompatibilityAsync(userId, targetUserId);

        if (existing != null)
        {
            existing.AgeCompatibility = compatibility.AgeCompatibility;
            existing.LocationCompatibility = compatibility.LocationCompatibility;
            existing.InterestCompatibility = compatibility.InterestCompatibility;
            existing.LifestyleCompatibility = compatibility.LifestyleCompatibility;
            existing.ProfileCompleteness = compatibility.ProfileCompleteness;
            existing.VerificationScore = compatibility.VerificationScore;
            existing.OverallScore = compatibility.OverallScore;
            existing.CalculatedAt = DateTime.UtcNow;
            existing.ExpiresAt = DateTime.UtcNow.AddHours(24);

            await _recommendationRepo.UpdateRecommendationScoreAsync(existing);
            return existing;
        }

        var newScore = new RecommendationScore
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RecommendedUserId = targetUserId,
            AgeCompatibility = compatibility.AgeCompatibility,
            LocationCompatibility = compatibility.LocationCompatibility,
            InterestCompatibility = compatibility.InterestCompatibility,
            LifestyleCompatibility = compatibility.LifestyleCompatibility,
            ProfileCompleteness = compatibility.ProfileCompleteness,
            VerificationScore = compatibility.VerificationScore,
            OverallScore = compatibility.OverallScore
        };

        await _recommendationRepo.CreateRecommendationScoreAsync(newScore);
        return newScore;
    }

    public async Task PrefetchRecommendationsAsync(Guid userId)
    {
        // Get next 50 profiles and pre-calculate scores
        var skipped = await _skipRepo.GetSkippedProfilesAsync(userId);
        var allProfiles = await _recommendationRepo.GetAllProfilesExceptSkippedAsync(userId);

        foreach (var profileId in allProfiles.Take(50))
        {
            await SaveRecommendationScoreAsync(userId, profileId);
        }
    }

    private string FormatBreakdown(RecommendationScore score)
    {
        var reasons = new List<string>();
        if (score.AgeCompatibility > 70) reasons.Add("Similar age");
        if (score.LocationCompatibility > 70) reasons.Add("Close location");
        if (score.InterestCompatibility > 70) reasons.Add("Shared interests");
        if (score.VerificationScore > 80) reasons.Add("Verified profile");

        return string.Join(", ", reasons);
    }
}

public class PreferenceMatchingService : IPreferenceMatchingService
{
    public decimal CalculateAgeCompatibility(int userAge, UserPreferences preferences)
    {
        if (userAge < preferences.MinAgePreference || userAge > preferences.MaxAgePreference)
            return 0;

        var midpoint = (preferences.MinAgePreference + preferences.MaxAgePreference) / 2m;
        var distance = Math.Abs(userAge - midpoint);
        var maxDistance = (preferences.MaxAgePreference - preferences.MinAgePreference) / 2m;

        return Math.Max(0, 100 - (distance / maxDistance * 100));
    }

    public decimal CalculateLocationCompatibility(string userLocation, UserPreferences preferences)
    {
        // Calculate based on distance
        // In real implementation, calculate distance between coordinates
        return 75m; // Placeholder
    }

    public decimal CalculateLifestyleCompatibility(Guid userId, UserPreferences preferences)
    {
        // Calculate based on smoking, drinking, children preferences
        return 70m; // Placeholder
    }

    public decimal CalculateHeightCompatibility(int userHeight, UserPreferences preferences)
    {
        if (preferences.MinHeightPreference == null || preferences.MaxHeightPreference == null)
            return 100m; // No preference = perfect match

        if (userHeight < preferences.MinHeightPreference || userHeight > preferences.MaxHeightPreference)
            return 0;

        return 90m;
    }
}

public class InterestMatchingService : IInterestMatchingService
{
    private readonly IPreferenceRepository _preferenceRepo;

    public InterestMatchingService(IPreferenceRepository preferenceRepo)
    {
        _preferenceRepo = preferenceRepo;
    }

    public decimal CalculateInterestCompatibility(Guid userId, Guid targetUserId)
    {
        var matching = GetMatchingInterests(userId, targetUserId);
        var commonCategories = GetCommonInterestCategories(userId, targetUserId);

        if (matching.Count == 0) return 30m;
        if (matching.Count <= 2) return 60m;
        if (matching.Count <= 5) return 80m;
        return 100m;
    }

    public List<string> GetMatchingInterests(Guid userId, Guid targetUserId)
    {
        // Get common interests between two users
        // Placeholder implementation
        return new List<string> { "Travel", "Music", "Sports" };
    }

    public List<string> GetCommonInterestCategories(Guid userId, Guid targetUserId)
    {
        // Get common interest categories
        return new List<string> { "Arts", "Sports", "Technology" };
    }
}

public class ProfileScoreService : IProfileScoreService
{
    public decimal CalculateProfileCompletenessScore(Guid userId)
    {
        // Calculate based on profile fields filled
        // Photo, bio, interests, verification, etc.
        return 80m; // Placeholder
    }

    public decimal CalculateVerificationScore(Guid userId)
    {
        // Check if phone verified, email verified, ID verified
        return 75m; // Placeholder
    }

    public decimal CalculateOverallScore(CompatibilityScoreDto compatibility)
    {
        // Weighted average of all scores
        var weights = new Dictionary<string, decimal>
        {
            { "age", 0.20m },
            { "location", 0.15m },
            { "interest", 0.35m },
            { "lifestyle", 0.10m },
            { "completeness", 0.10m },
            { "verification", 0.10m }
        };

        var score = 
            (compatibility.AgeCompatibility * weights["age"]) +
            (compatibility.LocationCompatibility * weights["location"]) +
            (compatibility.InterestCompatibility * weights["interest"]) +
            (compatibility.LifestyleCompatibility * weights["lifestyle"]) +
            (compatibility.ProfileCompleteness * weights["completeness"]) +
            (compatibility.VerificationScore * weights["verification"]);

        return Math.Min(100, Math.Max(0, score));
    }
}
