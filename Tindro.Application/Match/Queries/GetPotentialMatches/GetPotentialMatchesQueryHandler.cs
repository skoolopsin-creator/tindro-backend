using Tindro.Application.Common;
using MediatR;
using Tindro.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Match;
using Tindro.Application.Common.Extensions;

namespace Tindro.Application.Match.Queries.GetPotentialMatches
{
    public class GetPotentialMatchesQueryHandler : MediatR.IRequestHandler<GetPotentialMatchesQuery, Ardalis.Result.Result<System.Collections.Generic.List<Tindro.Application.Match.PotentialMatchDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ICurrentUserService _currentUser;

        public GetPotentialMatchesQueryHandler(
            IUserRepository userRepository,
            IMatchRepository matchRepository,
            ICurrentUserService currentUser)
        {
            _userRepository = userRepository;
            _matchRepository = matchRepository;
            _currentUser = currentUser;
        }

        public async Task<Ardalis.Result.Result<System.Collections.Generic.List<Tindro.Application.Match.PotentialMatchDto>>> Handle(GetPotentialMatchesQuery request, CancellationToken ct)
        {
            var currentUserId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            var currentProfile = await _userRepository.GetProfileAsync(currentUserId, ct);
            if (currentProfile == null)
                return Ardalis.Result.Result<
          System.Collections.Generic.List<Tindro.Application.Match.PotentialMatchDto>
      >.Error(DomainErrors.Profile.NotFound);

            // Get users we already interacted with
            var excludedIds = await _matchRepository.GetSwipedUserIdsAsync(currentUserId, ct);
            excludedIds.Add(currentUserId); // exclude self

            var potentialUsers = await _userRepository.GetPotentialMatchesAsync(
                currentProfile,
                excludedIds,
                request.Page,
                request.PageSize,
                request.MaxDistanceKm ?? double.MaxValue,
                ct);

            var dtos = potentialUsers.Select(u => new PotentialMatchDto
            {
                UserId = u.Id.ToString(),
                Name = u.Profile?.Name ?? "User",
                Age = u.Profile?.BirthDate.CalculateAge() ?? 0,
                MainPhotoUrl = u.Profile?.GetMainPhotoUrl(),
                Bio = u.Profile?.Bio,
                DistanceKm = 9999,
                Interests = u.Profile?.Interests?.ToArray(),
                LastActive = u.LastActive
            })
            .OrderBy(x => x.DistanceKm)
            .ThenByDescending(x => x.LastActive)
            .ToList();

            return Ardalis.Result.Result.Success(dtos);
        }
    }
}
