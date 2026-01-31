using MediatR;
using Tindro.Application.Common.Interfaces;
using Tindro.Application.Feed.Dtos;

public class GetPostByIdQueryHandler : IRequestHandler<GetPostByIdQuery, PostDto>
{
    private readonly IPostRepository _repo;

    public GetPostByIdQueryHandler(IPostRepository repo)
    {
        _repo = repo;
    }

    public async Task<PostDto> Handle(GetPostByIdQuery request, CancellationToken ct)
    {
        var post = await _repo.GetByIdAsync(request.PostId, ct);

        return new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            MediaUrl = post.MediaUrl,
            UserId = post.UserId,
            LikeCount = post.LikeCount,
            CreatedAt = post.CreatedAt
        };
    }
}
