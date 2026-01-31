using MediatR;
using Tindro.Application.Common.Interfaces;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IPostRepository _repo;

    public DeletePostCommandHandler(IPostRepository repo)
    {
        _repo = repo;
    }

    public async Task Handle(DeletePostCommand request, CancellationToken ct)
    {
        await _repo.DeleteAsync(request.PostId, request.UserId, ct);
    }
}
