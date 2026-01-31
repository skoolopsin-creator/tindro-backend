using MediatR;
using Tindro.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common;
using Tindro.Application.Common.Extensions;
using Tindro.Shared.Enums;

namespace Tindro.Application.Match.Commands.Swipe
{
    public class SwipeCommandHandler : MediatR.IRequestHandler<SwipeCommand, Ardalis.Result.Result<Tindro.Application.Match.Commands.Swipe.SwipeResultDto?>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMatchRepository _matchRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMediator _mediator;           // for publishing events

        public SwipeCommandHandler(
            IUserRepository userRepository,
            IMatchRepository matchRepository,
            ICurrentUserService currentUser,
            IMediator mediator)
        {
            _userRepository = userRepository;
            _matchRepository = matchRepository;
            _currentUser = currentUser;
            _mediator = mediator;
        }

        public async Task<Ardalis.Result.Result<Tindro.Application.Match.Commands.Swipe.SwipeResultDto?>> Handle(SwipeCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUser.UserId
                ?? throw new UnauthorizedAccessException();

            if (currentUserId == request.TargetUserId)
                return Ardalis.Result.Result<Tindro.Application.Match.Commands.Swipe.SwipeResultDto?>.Error(DomainErrors.Swipe.CannotSwipeYourself);

            var targetUser = await _userRepository.GetByIdAsync(request.TargetUserId, cancellationToken);
            if (targetUser is null)
                return Ardalis.Result.Result<Tindro.Application.Match.Commands.Swipe.SwipeResultDto?>.Error(DomainErrors.User.NotFound);

            // Check daily swipe limit / subscription status etc...
            // (you can add validation behavior or domain service for this)

            var existingSwipe = await _matchRepository.GetSwipeAsync(
                currentUserId, request.TargetUserId, cancellationToken);

            if (existingSwipe != null)
                return Ardalis.Result.Result<Tindro.Application.Match.Commands.Swipe.SwipeResultDto?>.Error(DomainErrors.Swipe.AlreadySwiped);

            var swipe = new Tindro.Domain.Match.Swipe(
                fromUserId: currentUserId,
                toUserId: request.TargetUserId,
                direction: request.Direction,
                swipedAt: DateTime.UtcNow);

            await _matchRepository.AddSwipeAsync(swipe, cancellationToken);

            Tindro.Domain.Match.Match? newMatch = null;

            // Check for mutual like → create match
            if (request.Direction == Tindro.Shared.Enums.SwipeDirection.Like || request.Direction == Tindro.Shared.Enums.SwipeDirection.SuperLike)
            {
                var reverseSwipe = await _matchRepository.GetSwipeAsync(
                    request.TargetUserId, currentUserId, cancellationToken);

                if (reverseSwipe != null && reverseSwipe.IsLike)
                {
                    newMatch = Tindro.Domain.Match.Match.CreateMutual(
                        userId1: currentUserId,
                        userId2: request.TargetUserId,
                        createdAt: DateTime.UtcNow);

                    await _matchRepository.AddMatchAsync(newMatch, cancellationToken);

                    // Publish domain event (for notifications, chat creation, etc)
                    await _mediator.Publish(new Tindro.Application.Match.Events.MatchCreatedEvent(newMatch.Id.ToString(), currentUserId, request.TargetUserId));
                }
            }

            var result = new SwipeResultDto(
                IsMatch: newMatch != null,
                Match: newMatch != null ? new MatchDto
                {
                    MatchId = newMatch.Id.ToString(),
                    MatchedWithUserId = request.TargetUserId,
                    MatchedWithName = targetUser.Profile?.Name ?? "User",
                    MatchedWithMainPhotoUrl = targetUser.Profile?.GetMainPhotoUrl(),
                    MatchedAt = newMatch.CreatedAt,
                    IsNew = true
                } : null
            );

            return Ardalis.Result.Result.Success(result);
        }
    }
}
