using Tindro.Domain.Feed;

namespace Tindro.Application.Common.Interfaces;

public interface IPostRepository
{
    Task AddAsync(Post post, CancellationToken ct);

    Task<Post> GetByIdAsync(Guid postId, CancellationToken ct);


    Task DeleteAsync(Guid postId, Guid userId, CancellationToken ct);

    Task LikeAsync(Guid postId, Guid userId, CancellationToken ct);
    Task UnlikeAsync(Guid postId, Guid userId, CancellationToken ct);
    Task AddCommentAsync(Guid postId, Guid userId, string text, CancellationToken ct);
    Task<List<PostComment>> GetCommentsAsync(Guid postId, CancellationToken ct);

}
