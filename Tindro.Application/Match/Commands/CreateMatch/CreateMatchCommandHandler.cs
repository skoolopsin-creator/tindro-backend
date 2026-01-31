// Tindro.Application/Match/Commands/CreateMatch/CreateMatchCommandHandler.cs
using MediatR;
using Tindro.Application.Common.Interfaces;
using Tindro.Domain.Match;
using Tindro.Application.Common.Models;
using Tindro.Application.Common;
using Tindro.Application.Match.Events;
using Tindro.Application.Common.Extensions;

namespace Tindro.Application.Match.Commands.CreateMatch;

public class CreateMatchCommandHandler : MediatR.IRequestHandler<CreateMatchCommand, Ardalis.Result.Result<Tindro.Application.Common.Models.MatchDto>>
{
    private readonly IMatchRepository _matchRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMediator _mediator;

    public CreateMatchCommandHandler(
        IMatchRepository matchRepository,
        IUserRepository userRepository,
        IMediator mediator)
    {
        _matchRepository = matchRepository;
        _userRepository = userRepository;
        _mediator = mediator;
    }

    public async Task<Ardalis.Result.Result<Tindro.Application.Common.Models.MatchDto>> Handle(CreateMatchCommand request, CancellationToken cancellationToken)
    {
        // Basic validation
        if (request.UserId1 == request.UserId2)
            return Ardalis.Result.Result<Tindro.Application.Common.Models.MatchDto>.Error(DomainErrors.Match.CannotMatchWithYourself);

        // Check if match already exists
        var existingMatch = await _matchRepository.GetMatchBetweenUsersAsync(
            request.UserId1, request.UserId2, cancellationToken);

        if (existingMatch != null)
            return Ardalis.Result.Result<Tindro.Application.Common.Models.MatchDto>.Error(DomainErrors.Match.AlreadyExists);

        // Verify both users exist
        var user1 = await _userRepository.GetByIdAsync(request.UserId1, cancellationToken);
        var user2 = await _userRepository.GetByIdAsync(request.UserId2, cancellationToken);

        if (user1 == null || user2 == null)
            return Ardalis.Result.Result<Tindro.Application.Common.Models.MatchDto>.Error(DomainErrors.User.NotFound);

        var createdAt = request.CreatedAt ?? DateTime.UtcNow;

        var match = Tindro.Domain.Match.Match.CreateMutual(
            userId1: request.UserId1,
            userId2: request.UserId2,
            createdAt: createdAt);

        await _matchRepository.AddMatchAsync(match, cancellationToken);

        // Publish event for notifications, chat room creation, etc.
        await _mediator.Publish(new MatchCreatedEvent(match.Id.ToString(), request.UserId1, request.UserId2), cancellationToken);

        // Return DTO
        var matchedWith = request.UserId1 == user1.Id.ToString() ? user2 : user1;

        var dto = new Tindro.Application.Common.Models.MatchDto
        {
            MatchId = match.Id.ToString(),
            MatchedWithUserId = matchedWith.Id.ToString(),
            MatchedWithName = matchedWith.Profile?.Name ?? "User",
            MatchedWithMainPhotoUrl = matchedWith.Profile?.GetMainPhotoUrl(),
            MatchedAt = match.CreatedAt,
            IsNew = true
        };

        return Ardalis.Result.Result.Success(dto);
    }
}