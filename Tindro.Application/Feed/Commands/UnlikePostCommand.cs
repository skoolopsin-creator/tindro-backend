using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Feed.Commands
{
    public record UnlikePostCommand(Guid PostId, Guid UserId) : IRequest;

}
