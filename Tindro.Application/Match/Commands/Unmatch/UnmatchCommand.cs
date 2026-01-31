// Tindro.Application/Match/Commands/Unmatch/UnmatchCommand.cs
using MediatR;
using Tindro.Application.Common.Models;
using Tindro.Application.Common;

namespace Tindro.Application.Match.Commands.Unmatch;

public record UnmatchCommand(
    string MatchId
) : MediatR.IRequest<Ardalis.Result.Result>;