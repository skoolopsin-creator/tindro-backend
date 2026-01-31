using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Feed.Commands.Comments
{
    public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand>
    {
        private readonly IPostRepository _repo;
        public AddCommentCommandHandler(IPostRepository repo) => _repo = repo;

        public async Task Handle(AddCommentCommand cmd, CancellationToken ct)
            => await _repo.AddCommentAsync(cmd.PostId, cmd.UserId, cmd.Text, ct);
    }

}
