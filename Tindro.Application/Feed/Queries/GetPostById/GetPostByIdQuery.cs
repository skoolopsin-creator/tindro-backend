using MediatR;
using Tindro.Application.Feed.Dtos;

public record GetPostByIdQuery(Guid PostId) : IRequest<PostDto>;
