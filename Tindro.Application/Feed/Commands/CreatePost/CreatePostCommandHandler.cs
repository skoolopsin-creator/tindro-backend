using MediatR;
using Tindro.Application.Common.Interfaces;
using Tindro.Application.Feed.Dtos;
using Tindro.Domain.Feed;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, PostDto>
{
    private readonly IPostRepository _repo;

    public CreatePostCommandHandler(IPostRepository repo)
    {
        _repo = repo;
    }

    public async Task<PostDto> Handle(CreatePostCommand request, CancellationToken ct)
    {
        var post = new Post(request.UserId, request.Content, request.MediaUrl);
        await _repo.AddAsync(post, ct);

        return new PostDto
        {
            Id = post.Id,
            Content = post.Content,
            MediaUrl = post.MediaUrl,
            UserId = post.UserId,
            LikeCount = 0,
            CreatedAt = post.CreatedAt
        };
    }
}
