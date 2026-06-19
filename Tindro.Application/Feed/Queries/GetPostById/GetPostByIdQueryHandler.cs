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

    public async Task<PostDto> Handle(
        GetPostByIdQuery request,
        CancellationToken ct)
    {
        var post = await _repo.GetByIdAsync(request.PostId, ct);

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