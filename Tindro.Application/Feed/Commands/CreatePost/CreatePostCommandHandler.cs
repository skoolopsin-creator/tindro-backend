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

    public async Task<PostDto> Handle(
        CreatePostCommand request,
        CancellationToken ct)
    {
        var post = new Post(
            request.UserId,
            request.Title,
            request.Description,
            request.MediaUrl,
          request.Tags == null
        ? null
        : string.Join(",", request.Tags)
        );

        await _repo.AddAsync(post, ct);

        return new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,

            Title = post.Title,
            Description = post.Description,
            Tags = string.IsNullOrWhiteSpace(post.Tags)
                    ? new List<string>()
                    : post.Tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList(),

            MediaUrl = post.MediaUrl,

            LikeCount = post.LikeCount,
            CommentCount = post.CommentCount,
            ShareCount = post.ShareCount,

            CreatedAt = post.CreatedAt
        };
    }
}