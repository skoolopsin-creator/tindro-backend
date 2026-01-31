using MediatR;
using Tindro.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common;

namespace Tindro.Application.Match.Queries.GetMatchesForUser
{
    public class GetMatchesForUserQueryHandler : MediatR.IRequestHandler<GetMatchesForUserQuery, Ardalis.Result.Result<System.Collections.Generic.List<Tindro.Application.Common.Models.MatchDto>>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICurrentUserService _currentUser;

        public GetMatchesForUserQueryHandler(
            IMatchRepository matchRepository,
            IUserRepository userRepository,
            ICurrentUserService currentUser)
        {
            _matchRepository = matchRepository;
            _userRepository = userRepository;
            _currentUser = currentUser;
        }

        public async Task<Ardalis.Result.Result<System.Collections.Generic.List<Tindro.Application.Common.Models.MatchDto>>> Handle(GetMatchesForUserQuery request, CancellationToken ct)
        {
            var userId = _currentUser.UserId ?? throw new UnauthorizedAccessException();

            var matches = await _matchRepository.GetMatchesForUserAsync(userId, ct);

            var dtos = new System.Collections.Generic.List<Tindro.Application.Common.Models.MatchDto>();

            foreach (var match in matches)
            {
                var otherUserId = match.UserId1 == userId ? match.UserId2 : match.UserId1;
                var otherUser = await _userRepository.GetProfileSummaryAsync(otherUserId, ct);

                if (otherUser == null) continue;

                dtos.Add(new Tindro.Application.Common.Models.MatchDto
                {
                    MatchId = match.Id.ToString(),
                    MatchedWithUserId = otherUserId,
                    MatchedWithName = otherUser.Name,
                    MatchedWithMainPhotoUrl = otherUser.MainPhotoUrl,
                    MatchedAt = match.CreatedAt,
                    IsNew = match.CreatedAt > DateTime.UtcNow.AddHours(-24) // example
                });
            }

            return Ardalis.Result.Result.Success(dtos.OrderByDescending(x => x.MatchedAt).ToList());
        }
    }
}
