using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace Tindro.Application.Common.Interfaces
{
    public interface IMatchRepository
    {
        Task<Tindro.Domain.Match.Match?> GetMatchBetweenUsersAsync(string userId1, string userId2, CancellationToken ct = default);
        Task AddMatchAsync(Tindro.Domain.Match.Match match, CancellationToken ct = default);
        Task<List<Tindro.Domain.Match.Match>> GetMatchesForUserAsync(string userId, CancellationToken ct = default);

        Task<Tindro.Domain.Match.Swipe?> GetSwipeAsync(string fromUserId, string toUserId, CancellationToken ct = default);
        Task AddSwipeAsync(Tindro.Domain.Match.Swipe swipe, CancellationToken ct = default);

        Task<List<Tindro.Domain.Match.Boost>> GetActiveBoostsForUserAsync(string userId, DateTime asOf);
        Task<List<string>> GetSwipedUserIdsAsync(string userId, CancellationToken ct = default);

        Task<Tindro.Domain.Match.Match?> GetByIdAsync(string matchId, CancellationToken ct = default);
        Task DeleteMatchAsync(string matchId, CancellationToken ct = default);

        Task<int> DeleteOldSwipesAsync(DateTime cutoffDate, CancellationToken ct = default);
        Task<List<Tindro.Domain.Match.Boost>> GetExpiredBoostsAsync(DateTime now, CancellationToken ct = default);
        Task DeleteBoostAsync(Guid boostId, CancellationToken ct = default);
    }
}
