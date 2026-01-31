namespace Tindro.Application.Discovery.Services;

using Tindro.Application.Discovery.Interfaces;
using Tindro.Application.Discovery.Dtos;
using Tindro.Domain.Discovery;
using StackExchange.Redis;

public class FilterService : IFilterService
{
    private readonly IFilterRepository _filterRepository;
    private readonly IProfileMatcher _profileMatcher;
    private readonly IDatabase _redisDb;

    public FilterService(IFilterRepository filterRepository, IProfileMatcher profileMatcher, IConnectionMultiplexer redis)
    {
        _filterRepository = filterRepository;
        _profileMatcher = profileMatcher;
        _redisDb = redis.GetDatabase();
    }

    public async Task<FilterResultDto> ApplyFilterAsync(Guid userId, ApplyFilterRequestDto request)
    {
        FilterPreferencesDto preferences;
        
        if (request.FilterPreferences != null)
        {
            preferences = request.FilterPreferences;
        }
        else if (request.SavedFilterId.HasValue)
        {
            var savedFilter = await GetSavedFilterAsync(request.SavedFilterId.Value);
            preferences = savedFilter?.FilterPreferences ?? throw new InvalidOperationException("Saved filter not found");
        }
        else
        {
            var filterResult = await GetFilterPreferencesAsync(userId);
            preferences = filterResult ?? throw new InvalidOperationException("No filter preferences found");
        }

        if (preferences == null)
            throw new InvalidOperationException("No filter preferences found");

        // Load and validate preferences
        var filterEntity = MapToFilterPreferences(preferences, userId);
        var matchingProfiles = await FindMatchingProfilesAsync(userId, preferences);

        var page = request.Page;
        var pageSize = request.PageSize;
        var paginatedProfiles = matchingProfiles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Log filter application
        await _filterRepository.LogFilterApplicationAsync(new FilterApplicationHistory
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FilterPreferencesId = filterEntity.Id,
            ResultCount = matchingProfiles.Count,
            AppliedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });

        return new FilterResultDto
        {
            Profiles = new(), // Would populate with actual user data
            TotalCount = matchingProfiles.Count,
            Page = page,
            PageSize = pageSize,
            TotalPages = (matchingProfiles.Count + pageSize - 1) / pageSize,
            CriteriaMatched = matchingProfiles.Count,
            AppliedFilterName = preferences.Name,
            AppliedAt = DateTime.UtcNow
        };
    }

    public Task<List<Guid>> FindMatchingProfilesAsync(Guid userId, FilterPreferencesDto preferences)
    {
        var filterEntity = MapToFilterPreferences(preferences, userId);
        
        // Get all profiles and filter them
        var matchingProfiles = new List<Guid>();
        
        // In real implementation, this would query database for profiles
        // This is a placeholder for the core filtering logic
         return Task.FromResult(matchingProfiles);
    }

    public Task<FilterValidationResultDto> ValidateFilterAsync(FilterPreferencesDto preferences)
    {
        var errors = new List<string>();
        var warnings = new List<string>();

        // Validate age range
        if (preferences.MinAge < 18 || preferences.MinAge > preferences.MaxAge)
            errors.Add("Invalid age range");
        if (preferences.MaxAge > 150)
            warnings.Add("Maximum age exceeds 150");

        // Validate height range
        if (preferences.MinHeight.HasValue && preferences.MaxHeight.HasValue)
        {
            if (preferences.MinHeight > preferences.MaxHeight)
                errors.Add("Invalid height range");
        }

        // Validate distance
        if (preferences.MaxDistance.HasValue && preferences.MaxDistance <= 0)
            errors.Add("Distance must be positive");

        // Validate profile completion
        if (preferences.MinProfileCompletion.HasValue && (preferences.MinProfileCompletion < 0 || preferences.MinProfileCompletion > 100))
            errors.Add("Profile completion must be 0-100");

        return Task.FromResult(new FilterValidationResultDto
        {
            IsValid = errors.Count == 0,
            Errors = errors,
            Warnings = warnings,
            EstimatedResultCount = null,
            SuggestedOptimization = errors.Count > 0 ? null : "Filter is optimal"
       });
    }

    public async Task<FilterPreferencesDto> SaveFilterPreferencesAsync(Guid userId, FilterPreferencesDto preferencesDto)
    {
        var filterEntity = MapToFilterPreferences(preferencesDto, userId);
        var saved = await _filterRepository.CreateFilterPreferencesAsync(filterEntity);
        return MapToDto(saved);
    }

    public async Task<FilterPreferencesDto> GetFilterPreferencesAsync(Guid userId, string? filterName = null)
    {
        var filter = await _filterRepository.GetFilterPreferencesAsync(userId, filterName);
        return filter != null ? MapToDto(filter) : new FilterPreferencesDto();
    }

    public async Task<List<FilterPreferencesDto>> GetAllUserFiltersAsync(Guid userId)
    {
        var filters = await _filterRepository.GetAllUserFiltersAsync(userId);
        return filters.Select(MapToDto).ToList();
    }

    public async Task DeleteFilterAsync(Guid filterId)
    {
        await _filterRepository.DeleteFilterPreferencesAsync(filterId);
    }

    public async Task<SavedFilterDto> CreateSavedFilterAsync(Guid userId, string name, FilterPreferencesDto preferences)
    {
        var filterPrefs = MapToFilterPreferences(preferences, userId);
        var savedFilter = new SavedFilter
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = name,
            FilterPreferencesId = filterPrefs.Id,
            CreatedAt = DateTime.UtcNow
        };
        
        var created = await _filterRepository.CreateSavedFilterAsync(savedFilter);
        return MapSavedFilterToDto(created);
    }

    public async Task<List<SavedFilterDto>> GetSavedFiltersAsync(Guid userId)
    {
        var filters = await _filterRepository.GetUserSavedFiltersAsync(userId);
        return filters.Select(MapSavedFilterToDto).ToList();
    }

    public async Task<SavedFilterDto> GetSavedFilterAsync(Guid savedFilterId)
    {
        var filter = await _filterRepository.GetSavedFilterAsync(savedFilterId);
        return filter != null ? MapSavedFilterToDto(filter) : throw new KeyNotFoundException();
    }

    public async Task DeleteSavedFilterAsync(Guid savedFilterId)
    {
        await _filterRepository.DeleteSavedFilterAsync(savedFilterId);
    }

    public async Task<SavedFilterDto> SetDefaultFilterAsync(Guid userId, Guid savedFilterId)
    {
        var filter = await _filterRepository.GetSavedFilterAsync(savedFilterId);
        if (filter == null) throw new KeyNotFoundException();
        
        filter.IsDefault = true;
        var updated = await _filterRepository.UpdateSavedFilterAsync(filter);
        return MapSavedFilterToDto(updated);
    }

    public  Task<FilterResultDto> ApplyAdvancedFiltersAsync(Guid userId, List<FilterCriteriaDto> criteria, int page = 1, int pageSize = 20)
    {
        // Build complex filter from criteria
        var matchingProfiles = new List<Guid>();
        
        return Task.FromResult(new FilterResultDto
        {
            TotalCount = matchingProfiles.Count,
            Page = page,
            PageSize = pageSize,
            TotalPages = (matchingProfiles.Count + pageSize - 1) / pageSize,
            CriteriaMatched = criteria.Count,
            AppliedAt = DateTime.UtcNow
        });
    }

    public async Task<int> EstimateFilterResultsAsync(Guid userId, FilterPreferencesDto preferences)
    {
        var validation = await ValidateFilterAsync(preferences);
        if (!validation.IsValid) return 0;
        
        // Estimate based on filter properties
        return validation.EstimatedResultCount ?? 0;
    }

    public async Task<FilterAnalyticsDto> GetFilterAnalyticsAsync(Guid userId)
    {
        var history = await _filterRepository.GetFilterHistoryAsync(userId, 100);
        
        if (history.Count == 0)
        {
            return new FilterAnalyticsDto
            {
                TotalFiltersUsed = 0,
                AverageResultsPerFilter = 0,
                ConversionRate = 0,
                MatchRate = 0,
                TopFilters = new()
            };
        }

        var avgResults = history.Average(h => h.ResultCount);
        var avgViewed = history.Average(h => h.ProfilesViewed ?? 0);
        var avgMatches = history.Average(h => h.Matches ?? 0);

        return new FilterAnalyticsDto
        {
            TotalFiltersUsed = history.Count,
            AverageResultsPerFilter = (int)avgResults,
            ConversionRate = avgResults > 0 ? (int)(avgViewed / avgResults * 100) : 0,
            MatchRate = (int)avgMatches,
            TopFilters = new()
        };
    }

    public async Task<FilterUsageDto> GetFilterUsageStatsAsync(Guid userId, string filterName)
    {
        var filter = await _filterRepository.GetFilterPreferencesAsync(userId, filterName);
        if (filter == null) throw new KeyNotFoundException();
        
        var history = await _filterRepository.GetFilterHistoryAsync(userId);
        var filterHistory = history.FirstOrDefault();
        
        return new FilterUsageDto
        {
            FilterName = filterName,
            UsageCount = history.Count,
            ResultCount = filterHistory?.ResultCount ?? 0,
            ProfilesViewed = filterHistory?.ProfilesViewed ?? 0,
            Matches = filterHistory?.Matches ?? 0,
            LastUsed = filterHistory?.AppliedAt ?? DateTime.MinValue
        };
    }

    public async Task<List<UserProfileDto>> GetRecommendedProfilesAsync(Guid userId, FilterPreferencesDto? overrideFilter = null)
    {
        var preferences = overrideFilter ?? await GetFilterPreferencesAsync(userId);
        var matchingProfiles = await FindMatchingProfilesAsync(userId, preferences);
        
        return new(); // Would populate with actual profiles
    }

    private FilterPreferences MapToFilterPreferences(FilterPreferencesDto dto, Guid userId)
    {
        return new FilterPreferences
        {
            Id = dto.Id ?? Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            IsActive = dto.IsActive,
            MinAge = dto.MinAge,
            MaxAge = dto.MaxAge,
            MinHeight = dto.MinHeight,
            MaxHeight = dto.MaxHeight,
            MaxDistance = dto.MaxDistance,
            EducationLevel = dto.EducationLevel,
            SmokingPreference = dto.SmokingPreference,
            DrinkingPreference = dto.DrinkingPreference,
            ExerciseFrequency = dto.ExerciseFrequency,
            Religion = dto.Religion,
            RelationshipGoal = dto.RelationshipGoal,
            FamilyPlans = dto.FamilyPlans,
            FilterByPersonality = dto.FilterByPersonality,
            PersonalityTraits = dto.PersonalityTraits,
            FilterByInterests = dto.FilterByInterests,
            MinSharedInterests = dto.MinSharedInterests,
            RequireVerified = dto.RequireVerified,
            RequirePhotos = dto.RequirePhotos,
            MinPhotos = dto.MinPhotos,
            MinProfileCompletion = dto.MinProfileCompletion,
            SortBy = dto.SortBy,
            ShowOnlineOnly = dto.ShowOnlineOnly,
            ShowRecentlyActive = dto.ShowRecentlyActive,
            CreatedAt = DateTime.UtcNow
        };
    }

    private FilterPreferencesDto MapToDto(FilterPreferences filter)
    {
        return new FilterPreferencesDto
        {
            Id = filter.Id,
            Name = filter.Name,
            IsActive = filter.IsActive,
            MinAge = filter.MinAge,
            MaxAge = filter.MaxAge,
            MinHeight = filter.MinHeight,
            MaxHeight = filter.MaxHeight,
            MaxDistance = filter.MaxDistance,
            EducationLevel = filter.EducationLevel,
            SmokingPreference = filter.SmokingPreference,
            DrinkingPreference = filter.DrinkingPreference,
            ExerciseFrequency = filter.ExerciseFrequency,
            Religion = filter.Religion,
            RelationshipGoal = filter.RelationshipGoal,
            FamilyPlans = filter.FamilyPlans,
            FilterByPersonality = filter.FilterByPersonality,
            PersonalityTraits = filter.PersonalityTraits,
            FilterByInterests = filter.FilterByInterests,
            MinSharedInterests = filter.MinSharedInterests,
            RequireVerified = filter.RequireVerified,
            RequirePhotos = filter.RequirePhotos,
            MinPhotos = filter.MinPhotos,
            MinProfileCompletion = filter.MinProfileCompletion,
            SortBy = filter.SortBy,
            ShowOnlineOnly = filter.ShowOnlineOnly,
            ShowRecentlyActive = filter.ShowRecentlyActive
        };
    }

    private SavedFilterDto MapSavedFilterToDto(SavedFilter filter)
    {
        return new SavedFilterDto
        {
            Id = filter.Id,
            Name = filter.Name,
            Description = filter.Description,
            IsDefault = filter.IsDefault,
            UsageCount = filter.UsageCount,
            CreatedAt = filter.CreatedAt,
            LastAppliedAt = filter.LastAppliedAt,
            FilterPreferences = filter.FilterPreferences != null ? MapToDto(filter.FilterPreferences) : null
        };
    }
}

/// <summary>
/// Profile matcher implementation
/// </summary>
public class ProfileMatcher : IProfileMatcher
{
    public  Task<bool> MatchesFilterAsync(Guid profileUserId, FilterPreferences preferences)
    {
        // Check if profile matches all required criteria
        // In real implementation, would fetch user profile and validate against preferences
        return Task.FromResult(true);
    }

    public  Task<decimal> CalculateMatchScoreAsync(Guid profileUserId, FilterPreferences preferences)
    {
        // Calculate weighted match score 0-100
         return Task.FromResult(0m);
    }

    public  Task<List<string>> GetMatchFailureReasonsAsync(Guid profileUserId, FilterPreferences preferences)
    {
        var reasons = new List<string>();
        // Check each criterion and add failure reasons
        return Task.FromResult(reasons);
    }
}
