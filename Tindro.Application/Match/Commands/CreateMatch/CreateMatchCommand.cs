// Tindro.Application/Match/Commands/CreateMatch/CreateMatchCommand.cs
using MediatR;
using Tindro.Application.Common.Models;
using Tindro.Application.Common;

namespace Tindro.Application.Match.Commands.CreateMatch;

public record CreateMatchCommand(
    string UserId1,
    string UserId2,
    DateTime? CreatedAt = null
) : MediatR.IRequest<Ardalis.Result.Result<Tindro.Application.Common.Models.MatchDto>>;
