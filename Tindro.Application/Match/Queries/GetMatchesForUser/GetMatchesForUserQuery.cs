using Tindro.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Match.Queries.GetMatchesForUser
{
    public record GetMatchesForUserQuery : MediatR.IRequest<Ardalis.Result.Result<System.Collections.Generic.List<Tindro.Application.Common.Models.MatchDto>>>;
}
