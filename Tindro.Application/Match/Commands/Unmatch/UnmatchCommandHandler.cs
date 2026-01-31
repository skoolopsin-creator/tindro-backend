// Tindro.Application/Match/Commands/Unmatch/UnmatchCommandHandler.cs
using MediatR;
using Tindro.Application.Common.Interfaces;
using Tindro.Application.Common.Models;
using Tindro.Application.Common;

namespace Tindro.Application.Match.Commands.Unmatch;

public class UnmatchCommandHandler : MediatR.IRequestHandler<UnmatchCommand, Ardalis.Result.Result>
{
    private readonly IMatchRepository _matchRepository;
    private readonly ICurrentUserService _currentUser;

    public UnmatchCommandHandler(
        IMatchRepository matchRepository,
        ICurrentUserService currentUser)
    {
        _matchRepository = matchRepository;
        _currentUser = currentUser;
    }

    public async Task<Ardalis.Result.Result> Handle(UnmatchCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.UserId
            ?? throw new UnauthorizedAccessException();

        var match = await _matchRepository.GetByIdAsync(request.MatchId, cancellationToken);

        if (match == null)
            return Ardalis.Result.Result.Error(DomainErrors.Match.NotFound);

        if (!match.InvolvesUser(currentUserId))
            return Ardalis.Result.Result.Error(DomainErrors.Match.NotAuthorized);

        await _matchRepository.DeleteMatchAsync(match.Id.ToString(), cancellationToken);

        // Optional: publish UnmatchedEvent
        // await _mediator.Publish(new UnmatchedEvent(match.Id, currentUserId));

        return Ardalis.Result.Result.Success();
    }
}