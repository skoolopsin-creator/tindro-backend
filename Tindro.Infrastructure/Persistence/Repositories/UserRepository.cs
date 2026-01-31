using Microsoft.EntityFrameworkCore;
using Tindro.Application.Common.Interfaces;
using Tindro.Domain.Users;
using Tindro.Infrastructure.Persistence;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class UserRepository : IUserRepository
{
    private readonly CommandDbContext _db;

    public UserRepository(CommandDbContext db)
    {
        _db = db;
    }

    public async Task<User?> GetByPhoneAsync(string phone, CancellationToken ct = default)
        => await _db.Users.Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Phone == phone, ct);

    public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!Guid.TryParse(id, out var gid)) return null;
        return await _db.Users.Include(x => x.Profile)
            .FirstOrDefaultAsync(x => x.Id == gid, ct);
    }

    public async Task AddAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Tindro.Domain.Users.Profile?> GetProfileAsync(string userId, CancellationToken ct = default)
    {
        if (!Guid.TryParse(userId, out var gid)) return null;
        return await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == gid, ct);
    }

    public async Task<Tindro.Application.Common.Models.ProfileSummary?> GetProfileSummaryAsync(string userId, CancellationToken ct = default)
    {
        var user = await GetByIdAsync(userId, ct);
        if (user == null) return null;
        return new Tindro.Application.Common.Models.ProfileSummary
        {
            Id = user.Id.ToString(),
            Name = user.Profile?.Name ?? "User",
            MainPhotoUrl = user.Profile?.Photos?.FirstOrDefault()?.Url
        };
    }

    public async Task<List<User>> GetPotentialMatchesAsync(Tindro.Domain.Users.Profile currentProfile, List<string> excludedIds, int page, int pageSize, double maxDistanceKm, CancellationToken ct = default)
    {
        // Simple fallback: return recent users excluding excludedIds
        return await _db.Users.Include(u => u.Profile)
            .Where(u => !excludedIds.Contains(u.Id.ToString()))
            .OrderByDescending(u => u.LastActive)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
    }

    public Task<List<User>> GetPotentialMatchCandidatesAsync(User user, int limit, CancellationToken ct = default)
    {
        return _db.Users.Include(u => u.Profile)
            .OrderByDescending(u => u.LastActive)
            .Take(limit)
            .ToListAsync(ct);
    }

    public Task<List<User>> GetRecentlyActiveUsersAsync(DateTime lastActiveAfter, int take, CancellationToken ct = default)
    {
        return _db.Users.Include(u => u.Profile)
            .Where(u => u.LastActive > lastActiveAfter)
            .OrderByDescending(u => u.LastActive)
            .Take(take)
            .ToListAsync(ct);
    }

    public Task<List<User>> GetOnlineUsersAsync(TimeSpan activeWithin, CancellationToken ct = default)
    {
        var since = DateTime.UtcNow - activeWithin;
        return _db.Users.Include(u => u.Profile)
            .Where(u => u.LastActive > since)
            .ToListAsync(ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync(ct);
    }

    public Task<int> ApplyInactivityDecayAsync(DateTime inactiveSince, double decayFactor, CancellationToken ct = default)
    {
        // Not implemented: return 0
        return Task.FromResult(0);
    }
}
