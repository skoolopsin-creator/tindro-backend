using Tindro.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Match.Queries.GetPotentialMatches
{
    public record GetPotentialMatchesQuery(
    int Page = 1,
    int PageSize = 20,
    double? MaxDistanceKm = null
) : MediatR.IRequest<Ardalis.Result.Result<System.Collections.Generic.List<Tindro.Application.Match.PotentialMatchDto>>>;
}
