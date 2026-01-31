namespace Tindro.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Tindro.Application.Discovery.Interfaces;
using Tindro.Domain.Discovery;
using Tindro.Application.Verification.Interfaces;
public class FilterRepository : IFilterRepository
{
    private readonly CommandDbContext _context;

    public FilterRepository(CommandDbContext context)
    {
        _context = context;
    }

    // FilterPreferences operations
    public async Task<FilterPreferences?> GetFilterPreferencesAsync(Guid userId, string? filterName = null)
    {
        var query = _context.FilterPreferences.Where(f => f.UserId == userId);
        
        if (!string.IsNullOrEmpty(filterName))
            query = query.Where(f => f.Name == filterName);
        
        return await query.FirstOrDefaultAsync();
    }

    public async Task<List<FilterPreferences>> GetAllUserFiltersAsync(Guid userId)
    {
        return await _context.FilterPreferences
            .Where(f => f.UserId == userId && f.IsActive)
            .OrderByDescending(f => f.LastUsedAt)
            .ToListAsync();
    }

    public async Task<FilterPreferences> CreateFilterPreferencesAsync(FilterPreferences filterPreferences)
    {
        _context.FilterPreferences.Add(filterPreferences);
        await _context.SaveChangesAsync();
        return filterPreferences;
    }

    public async Task<FilterPreferences> UpdateFilterPreferencesAsync(FilterPreferences filterPreferences)
    {
        filterPreferences.UpdatedAt = DateTime.UtcNow;
        _context.FilterPreferences.Update(filterPreferences);
        await _context.SaveChangesAsync();
        return filterPreferences;
    }

    public async Task DeleteFilterPreferencesAsync(Guid filterId)
    {
        var filter = await _context.FilterPreferences.FindAsync(filterId);
        if (filter != null)
        {
            _context.FilterPreferences.Remove(filter);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<FilterPreferences?> GetDefaultFilterAsync(Guid userId)
    {
        return await _context.FilterPreferences
            .Where(f => f.UserId == userId && f.IsActive)
            .FirstOrDefaultAsync();
    }

    // FilterCriteria operations
    public async Task<List<FilterCriteria>> GetFilterCriteriaAsync(Guid filterPreferencesId)
    {
        return await _context.FilterCriteria
            .Where(c => c.FilterPreferencesId == filterPreferencesId)
            .OrderByDescending(c => c.Priority)
            .ToListAsync();
    }

    public async Task<FilterCriteria> AddFilterCriteriaAsync(FilterCriteria criteria)
    {
        _context.FilterCriteria.Add(criteria);
        await _context.SaveChangesAsync();
        return criteria;
    }

    public async Task RemoveFilterCriteriaAsync(Guid criteriaId)
    {
        var criteria = await _context.FilterCriteria.FindAsync(criteriaId);
        if (criteria != null)
        {
            _context.FilterCriteria.Remove(criteria);
            await _context.SaveChangesAsync();
        }
    }

    // SavedFilter operations
    public async Task<SavedFilter?> GetSavedFilterAsync(Guid savedFilterId)
    {
        return await _context.SavedFilters
            .Include(f => f.FilterPreferences)
            .FirstOrDefaultAsync(f => f.Id == savedFilterId);
    }

    public async Task<List<SavedFilter>> GetUserSavedFiltersAsync(Guid userId)
    {
        return await _context.SavedFilters
            .Include(f => f.FilterPreferences)
            .Where(f => f.UserId == userId)
            .OrderBy(f => f.IsDefault ? 0 : 1)
            .ThenByDescending(f => f.LastAppliedAt)
            .ToListAsync();
    }

    public async Task<SavedFilter> CreateSavedFilterAsync(SavedFilter savedFilter)
    {
        _context.SavedFilters.Add(savedFilter);
        await _context.SaveChangesAsync();
        return savedFilter;
    }

    public async Task<SavedFilter> UpdateSavedFilterAsync(SavedFilter savedFilter)
    {
        _context.SavedFilters.Update(savedFilter);
        await _context.SaveChangesAsync();
        return savedFilter;
    }

    public async Task DeleteSavedFilterAsync(Guid savedFilterId)
    {
        var filter = await _context.SavedFilters.FindAsync(savedFilterId);
        if (filter != null)
        {
            _context.SavedFilters.Remove(filter);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<SavedFilter?> GetDefaultSavedFilterAsync(Guid userId)
    {
        return await _context.SavedFilters
            .Include(f => f.FilterPreferences)
            .FirstOrDefaultAsync(f => f.UserId == userId && f.IsDefault);
    }

    // Application history
    public async Task<FilterApplicationHistory> LogFilterApplicationAsync(FilterApplicationHistory history)
    {
        _context.FilterApplicationHistories.Add(history);
        await _context.SaveChangesAsync();
        return history;
    }

    public async Task<List<FilterApplicationHistory>> GetFilterHistoryAsync(Guid userId, int limit = 10)
    {
        return await _context.FilterApplicationHistories
            .Where(h => h.UserId == userId)
            .OrderByDescending(h => h.AppliedAt)
            .Take(limit)
            .ToListAsync();
    }
}

/// <summary>
/// Verification repository implementation
/// </summary>
