using Tindro.Domain.Feed;
using Tindro.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Tindro.Infrastructure.Persistence;

public class PostRepository : IPostRepository
{
    private readonly CommandDbContext _db;

    public PostRepository(CommandDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Post post, CancellationToken ct)
    {
        _db.Posts.Add(post);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<Post> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _db.Posts.FirstAsync(x => x.Id == id, ct);
    }

    public async Task DeleteAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        var post = await _db.Posts.FirstAsync(x => x.Id == postId && x.UserId == userId, ct);
        _db.Posts.Remove(post);
        await _db.SaveChangesAsync(ct);
    }

    public async Task LikeAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        var exists = await _db.PostLikes.AnyAsync(pl => pl.PostId == postId && pl.UserId == userId, ct);
        if (exists) return;
        var like = new PostLike { PostId = postId, UserId = userId };
        _db.PostLikes.Add(like);

        var post = await _db.Posts.FirstAsync(p => p.Id == postId, ct);
        post.Like();
        _db.Posts.Update(post);

        await _db.SaveChangesAsync(ct);
    }

    public async Task UnlikeAsync(Guid postId, Guid userId, CancellationToken ct)
    {
        var like = await _db.PostLikes.FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId, ct);
        if (like == null) return;
        _db.PostLikes.Remove(like);

        var post = await _db.Posts.FirstAsync(p => p.Id == postId, ct);
        post.Unlike();
        _db.Posts.Update(post);

        await _db.SaveChangesAsync(ct);
    }

    public async Task AddCommentAsync(Guid postId, Guid userId, string text, CancellationToken ct)
    {
        var comment = new PostComment { PostId = postId, UserId = userId, Text = text, CreatedAt = DateTime.UtcNow };
        _db.PostComments.Add(comment);

        var post = await _db.Posts.FirstAsync(p => p.Id == postId, ct);
        post.AddComment();
        _db.Posts.Update(post);

        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<PostComment>> GetCommentsAsync(Guid postId, CancellationToken ct)
    {
        return await _db.PostComments.Where(c => c.PostId == postId).OrderBy(c => c.CreatedAt).ToListAsync(ct);
    }
}
