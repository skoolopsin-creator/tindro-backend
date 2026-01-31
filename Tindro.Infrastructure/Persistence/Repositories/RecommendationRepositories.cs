using Microsoft.EntityFrameworkCore;
using Tindro.Domain.Recommendations;
using Tindro.Application.Recommendations.Interfaces;
using Tindro.Infrastructure.Persistence;

namespace Tindro.Infrastructure.Persistence.Repositories;

public class RecommendationRepository : IRecommendationRepository
{
    private readonly QueryDbContext _context;

    public RecommendationRepository(QueryDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RecommendationScore>> GetRecommendationsAsync(Guid userId, int pageNumber, int pageSize)
    {
        return await _context.RecommendationScores
            .Where(r => r.UserId == userId && r.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(r => r.OverallScore)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<RecommendationScore?> GetRecommendationScoreAsync(Guid userId, Guid recommendedUserId)
    {
        return await _context.RecommendationScores
            .FirstOrDefaultAsync(r => r.UserId == userId && r.RecommendedUserId == recommendedUserId);
    }

    public async Task CreateRecommendationScoreAsync(RecommendationScore score)
    {
        await _context.RecommendationScores.AddAsync(score);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateRecommendationScoreAsync(RecommendationScore score)
    {
        _context.RecommendationScores.Update(score);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpiredScoresAsync()
    {
        await _context.RecommendationScores
            .Where(r => r.ExpiresAt <= DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }

    public async Task<List<Guid>> GetAllProfilesExceptSkippedAsync(Guid userId)
    {
        var skipped = await _context.SkipProfiles
            .Where(s => s.UserId == userId && s.ExpiresAt > DateTime.UtcNow)
            .Select(s => s.SkippedUserId)
            .ToListAsync();

        return await _context.RecommendationScores
            .Select(r => r.RecommendedUserId)
            .Where(id => !skipped.Contains(id))
            .ToListAsync();
    }

    public async Task<decimal> GetAverageScoreAsync(Guid userId)
    {
        return await _context.RecommendationScores
            .Where(r => r.UserId == userId)
            .AverageAsync(r => r.OverallScore);
    }
}

public class PreferenceRepository : IPreferenceRepository
{
    private readonly QueryDbContext _context;

    public PreferenceRepository(QueryDbContext context)
    {
        _context = context;
    }

    public async Task<UserPreferences?> GetUserPreferencesAsync(Guid userId)
    {
        return await _context.UserPreferences
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<UserPreferences?> GetOrCreatePreferencesAsync(Guid userId)
    {
        var existing = await GetUserPreferencesAsync(userId);
        if (existing != null) return existing;

        var newPreferences = new UserPreferences
        {
            Id = Guid.NewGuid(),
            UserId = userId
        };

        await CreatePreferencesAsync(newPreferences);
        return newPreferences;
    }

    public async Task CreatePreferencesAsync(UserPreferences preferences)
    {
        await _context.UserPreferences.AddAsync(preferences);
        await _context.SaveChangesAsync();
    }

    public async Task UpdatePreferencesAsync(UserPreferences preferences)
    {
        preferences.UpdatedAt = DateTime.UtcNow;
        _context.UserPreferences.Update(preferences);
        await _context.SaveChangesAsync();
    }

    public async Task<UserInterest?> GetInterestAsync(Guid userId, string interestName)
    {
        return await _context.UserInterests
            .FirstOrDefaultAsync(i => i.UserId == userId && i.InterestName == interestName);
    }

    public async Task<IEnumerable<UserInterest>> GetUserInterestsAsync(Guid userId)
    {
        return await _context.UserInterests
            .Where(i => i.UserId == userId)
            .ToListAsync();
    }

    public async Task AddInterestAsync(UserInterest interest)
    {
        await _context.UserInterests.AddAsync(interest);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveInterestAsync(Guid interestId)
    {
        var interest = await _context.UserInterests.FindAsync(interestId);
        if (interest != null)
        {
            _context.UserInterests.Remove(interest);
            await _context.SaveChangesAsync();
        }
    }
}

public class SkipRepository : ISkipRepository
{
    private readonly QueryDbContext _context;

    public SkipRepository(QueryDbContext context)
    {
        _context = context;
    }

    public async Task<SkipProfile?> GetSkipAsync(Guid userId, Guid skippedUserId)
    {
        return await _context.SkipProfiles
            .FirstOrDefaultAsync(s => s.UserId == userId && s.SkippedUserId == skippedUserId);
    }

    public async Task<IEnumerable<Guid>> GetSkippedProfilesAsync(Guid userId)
    {
        return await _context.SkipProfiles
            .Where(s => s.UserId == userId && s.ExpiresAt > DateTime.UtcNow)
            .Select(s => s.SkippedUserId)
            .ToListAsync();
    }

    public async Task<IEnumerable<SkipProfile>> GetActiveSkipsAsync(Guid userId)
    {
        return await _context.SkipProfiles
            .Where(s => s.UserId == userId && s.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();
    }

    public async Task CreateSkipAsync(SkipProfile skip)
    {
        await _context.SkipProfiles.AddAsync(skip);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteExpiredSkipsAsync()
    {
        await _context.SkipProfiles
            .Where(s => s.ExpiresAt <= DateTime.UtcNow)
            .ExecuteDeleteAsync();
    }

    public async Task DeleteSkipAsync(Guid userId, Guid skippedUserId)
    {
        var skip = await GetSkipAsync(userId, skippedUserId);
        if (skip != null)
        {
            _context.SkipProfiles.Remove(skip);
            await _context.SaveChangesAsync();
        }
    }
}
