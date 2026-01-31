using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tindro.Application.Common.Interfaces;
using Tindro.Application.Feed.Dtos;

namespace Tindro.Application.Feed.Commands.Comments
{
    public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, List<CommentDto>>
    {
        private readonly IPostRepository _repo;
        public GetCommentsQueryHandler(IPostRepository repo) => _repo = repo;

        public async Task<List<CommentDto>> Handle(GetCommentsQuery q, CancellationToken ct)
            => (await _repo.GetCommentsAsync(q.PostId, ct))
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Text = c.Text,
                    CreatedAt = c.CreatedAt
                }).ToList();
    }

}
