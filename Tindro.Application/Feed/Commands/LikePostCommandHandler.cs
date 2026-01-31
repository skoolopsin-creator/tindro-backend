using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common.Interfaces;

namespace Tindro.Application.Feed.Commands
{
    public class LikePostCommandHandler : IRequestHandler<LikePostCommand>
    {
        private readonly IPostRepository _repo;
        public LikePostCommandHandler(IPostRepository repo) => _repo = repo;

        public async Task Handle(LikePostCommand cmd, CancellationToken ct)
            => await _repo.LikeAsync(cmd.PostId, cmd.UserId, ct);
    }

}
