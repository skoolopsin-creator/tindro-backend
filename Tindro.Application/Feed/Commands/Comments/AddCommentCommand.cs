using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tindro.Application.Feed.Commands.Comments
{
    public record AddCommentCommand(Guid PostId, Guid UserId, string Text) : IRequest;

}
