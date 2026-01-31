using Tindro.Infrastructure.Persistence;

public class StoryExpiryJob
{
    private readonly CommandDbContext _db;

    public StoryExpiryJob(CommandDbContext db)
    {
        _db = db;
    }

    public void Run()
    {
        var expired = _db.Stories
            .Where(x => x.ExpiresAt < DateTime.UtcNow);

        _db.Stories.RemoveRange(expired);
        _db.SaveChanges();
    }
}
