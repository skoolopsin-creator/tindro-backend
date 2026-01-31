using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Feed.Dtos;

namespace Tindro.Application.Feed.Commands.Comments
{
    public record GetCommentsQuery(Guid PostId) : IRequest<List<CommentDto>>;

}
