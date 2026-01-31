using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Feed.Commands
{
    public class UnlikePostCommandHandler : IRequestHandler<UnlikePostCommand>
    {
        private readonly IPostRepository _repo;
        public UnlikePostCommandHandler(IPostRepository repo) => _repo = repo;

        public async Task Handle(UnlikePostCommand cmd, CancellationToken ct)
            => await _repo.UnlikeAsync(cmd.PostId, cmd.UserId, ct);
    }

}
