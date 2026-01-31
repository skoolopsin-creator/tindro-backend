namespace Tindro.Application.Discovery.Interfaces;

using Tindro.Application.Discovery.Dtos;
using Tindro.Domain.Discovery;

/// <summary>
/// Filter repository interface
/// </summary>
public interface IFilterRepository
{
    // FilterPreferences operations
    Task<FilterPreferences?> GetFilterPreferencesAsync(Guid userId, string? filterName = null);
    Task<List<FilterPreferences>> GetAllUserFiltersAsync(Guid userId);
    Task<FilterPreferences> CreateFilterPreferencesAsync(FilterPreferences filterPreferences);
    Task<FilterPreferences> UpdateFilterPreferencesAsync(FilterPreferences filterPreferences);
    Task DeleteFilterPreferencesAsync(Guid filterId);
    Task<FilterPreferences?> GetDefaultFilterAsync(Guid userId);

    // FilterCriteria operations
    Task<List<FilterCriteria>> GetFilterCriteriaAsync(Guid filterPreferencesId);
    Task<FilterCriteria> AddFilterCriteriaAsync(FilterCriteria criteria);
    Task RemoveFilterCriteriaAsync(Guid criteriaId);

    // SavedFilter operations
    Task<SavedFilter?> GetSavedFilterAsync(Guid savedFilterId);
    Task<List<SavedFilter>> GetUserSavedFiltersAsync(Guid userId);
    Task<SavedFilter> CreateSavedFilterAsync(SavedFilter savedFilter);
    Task<SavedFilter> UpdateSavedFilterAsync(SavedFilter savedFilter);
    Task DeleteSavedFilterAsync(Guid savedFilterId);
    Task<SavedFilter?> GetDefaultSavedFilterAsync(Guid userId);

    // Application history
    Task<FilterApplicationHistory> LogFilterApplicationAsync(FilterApplicationHistory history);
    Task<List<FilterApplicationHistory>> GetFilterHistoryAsync(Guid userId, int limit = 10);
}

/// <summary>
/// Filter service interface
/// </summary>
public interface IFilterService
{
    // Apply and query filters
    Task<FilterResultDto> ApplyFilterAsync(Guid userId, ApplyFilterRequestDto request);
    Task<List<Guid>> FindMatchingProfilesAsync(Guid userId, FilterPreferencesDto preferences);
    Task<FilterValidationResultDto> ValidateFilterAsync(FilterPreferencesDto preferences);

    // Manage preferences
    Task<FilterPreferencesDto> SaveFilterPreferencesAsync(Guid userId, FilterPreferencesDto preferencesDto);
    Task<FilterPreferencesDto> GetFilterPreferencesAsync(Guid userId, string? filterName = null);
    Task<List<FilterPreferencesDto>> GetAllUserFiltersAsync(Guid userId);
    Task DeleteFilterAsync(Guid filterId);

    // Saved filters
    Task<SavedFilterDto> CreateSavedFilterAsync(Guid userId, string name, FilterPreferencesDto preferences);
    Task<List<SavedFilterDto>> GetSavedFiltersAsync(Guid userId);
    Task<SavedFilterDto> GetSavedFilterAsync(Guid savedFilterId);
    Task DeleteSavedFilterAsync(Guid savedFilterId);
    Task<SavedFilterDto> SetDefaultFilterAsync(Guid userId, Guid savedFilterId);

    // Advanced filtering
    Task<FilterResultDto> ApplyAdvancedFiltersAsync(Guid userId, List<FilterCriteriaDto> criteria, int page = 1, int pageSize = 20);
    Task<int> EstimateFilterResultsAsync(Guid userId, FilterPreferencesDto preferences);

    // Analytics
    Task<FilterAnalyticsDto> GetFilterAnalyticsAsync(Guid userId);
    Task<FilterUsageDto> GetFilterUsageStatsAsync(Guid userId, string filterName);

    // Recommendations based on filters
    Task<List<UserProfileDto>> GetRecommendedProfilesAsync(Guid userId, FilterPreferencesDto? overrideFilter = null);
}

/// <summary>
/// Profile match evaluation interface
/// </summary>
public interface IProfileMatcher
{
    Task<bool> MatchesFilterAsync(Guid profileUserId, FilterPreferences preferences);
    Task<decimal> CalculateMatchScoreAsync(Guid profileUserId, FilterPreferences preferences);
    Task<List<string>> GetMatchFailureReasonsAsync(Guid profileUserId, FilterPreferences preferences);
}
