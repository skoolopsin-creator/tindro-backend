using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tindro.Domain.Users;

namespace Tindro.Application.Common.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByPhoneAsync(string phone, CancellationToken ct = default);
        Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);

        Task<Tindro.Domain.Users.Profile?> GetProfileAsync(string userId, CancellationToken ct = default);
        Task<Tindro.Application.Common.Models.ProfileSummary?> GetProfileSummaryAsync(string userId, CancellationToken ct = default);

        Task<List<User>> GetPotentialMatchesAsync(
            Tindro.Domain.Users.Profile currentProfile,
            List<string> excludedIds,
            int page,
            int pageSize,
            double maxDistanceKm,
            CancellationToken ct = default);

        Task<List<User>> GetPotentialMatchCandidatesAsync(User user, int limit, CancellationToken ct = default);

        Task<List<User>> GetRecentlyActiveUsersAsync(DateTime lastActiveAfter, int take, CancellationToken ct = default);
        Task<List<User>> GetOnlineUsersAsync(TimeSpan activeWithin, CancellationToken ct = default);

        Task UpdateAsync(User user, CancellationToken ct = default);

        Task<int> ApplyInactivityDecayAsync(DateTime inactiveSince, double decayFactor, CancellationToken ct = default);
    }
}
