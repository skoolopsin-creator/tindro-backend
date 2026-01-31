using MediatR;

public record DeletePostCommand(Guid PostId, Guid UserId) : IRequest;
