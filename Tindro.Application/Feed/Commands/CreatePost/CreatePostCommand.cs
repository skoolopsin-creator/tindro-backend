using MediatR;
using Tindro.Application.Feed.Dtos;

public record CreatePostCommand(
    string Title,
    string Description,
    string? MediaUrl,
     List<string>? Tags,
    Guid UserId
) : IRequest<PostDto>;
