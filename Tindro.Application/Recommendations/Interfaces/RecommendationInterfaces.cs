using Tindro.Domain.Recommendations;
using Tindro.Application.Recommendations.Dtos;
using Tindro.Domain.Common;

namespace Tindro.Application.Recommendations.Interfaces;

public interface IRecommendationRepository
{
    Task<IEnumerable<RecommendationScore>> GetRecommendationsAsync(Guid userId, int pageNumber, int pageSize);
    Task<RecommendationScore?> GetRecommendationScoreAsync(Guid userId, Guid recommendedUserId);
    Task CreateRecommendationScoreAsync(RecommendationScore score);
    Task UpdateRecommendationScoreAsync(RecommendationScore score);
    Task DeleteExpiredScoresAsync();
    Task<List<Guid>> GetAllProfilesExceptSkippedAsync(Guid userId);
    Task<decimal> GetAverageScoreAsync(Guid userId);
}

public interface IPreferenceRepository
{
    Task<UserPreferences?> GetUserPreferencesAsync(Guid userId);
    Task<UserPreferences?> GetOrCreatePreferencesAsync(Guid userId);
    Task CreatePreferencesAsync(UserPreferences preferences);
    Task UpdatePreferencesAsync(UserPreferences preferences);
    Task<UserInterest?> GetInterestAsync(Guid userId, Guid interestId);
    Task<IEnumerable<UserInterest>> GetUserInterestsAsync(Guid userId);
    Task AddInterestAsync(UserInterest interest);
    Task RemoveInterestAsync(Guid interestId);

    Task<bool> ExistsAsync(Guid userId, Guid interestId);
    
}

public interface ISkipRepository
{
    Task<SkipProfile?> GetSkipAsync(Guid userId, Guid skippedUserId);
    Task<IEnumerable<Guid>> GetSkippedProfilesAsync(Guid userId);
    Task<IEnumerable<SkipProfile>> GetActiveSkipsAsync(Guid userId);
    Task CreateSkipAsync(SkipProfile skip);
    Task DeleteExpiredSkipsAsync();
    Task DeleteSkipAsync(Guid userId, Guid skippedUserId);
}

public interface IRecommendationService
{
    Task<IEnumerable<RecommendationDto>> GetRecommendationsAsync(Guid userId, RecommendationFilterDto filter);
    Task<CompatibilityScoreDto> CalculateCompatibilityAsync(Guid userId, Guid targetUserId);
    Task<RecommendationScore> SaveRecommendationScoreAsync(Guid userId, Guid targetUserId);
    Task PrefetchRecommendationsAsync(Guid userId);
}

public interface IPreferenceMatchingService
{
    decimal CalculateAgeCompatibility(int userAge, UserPreferences preferences);
    decimal CalculateLocationCompatibility(string userLocation, UserPreferences preferences);
    decimal CalculateLifestyleCompatibility(Guid userId, UserPreferences preferences);
    decimal CalculateHeightCompatibility(int userHeight, UserPreferences preferences);

}

public interface IInterestMatchingService
{
    decimal CalculateInterestCompatibility(Guid userId, Guid targetUserId);
    List<string> GetMatchingInterests(Guid userId, Guid targetUserId);
    List<string> GetCommonInterestCategories(Guid userId, Guid targetUserId);
}

public interface IProfileScoreService
{
    decimal CalculateProfileCompletenessScore(Guid userId);
    decimal CalculateVerificationScore(Guid userId);
    decimal CalculateOverallScore(CompatibilityScoreDto compatibility);
}

public interface IInterestRepository
{
    Task<Interest?> GetByIdAsync(Guid id);

}