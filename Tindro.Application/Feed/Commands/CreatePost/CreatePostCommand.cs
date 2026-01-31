using MediatR;
using Tindro.Application.Feed.Dtos;

public record CreatePostCommand(string Content, string? MediaUrl, Guid UserId) : IRequest<PostDto>;
