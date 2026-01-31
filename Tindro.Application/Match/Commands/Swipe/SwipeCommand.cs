using MediatR;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common;
using Tindro.Shared.Enums;

namespace Tindro.Application.Match.Commands.Swipe
{
    public record SwipeCommand(
    string TargetUserId,
    SwipeDirection Direction  // Like / Dislike / SuperLike
        ) : MediatR.IRequest<Ardalis.Result.Result<Tindro.Application.Match.Commands.Swipe.SwipeResultDto?>>;

    public record SwipeResultDto(
        bool IsMatch,
        MatchDto? Match
    );
}
