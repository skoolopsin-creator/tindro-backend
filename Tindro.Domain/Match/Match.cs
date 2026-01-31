namespace Tindro.Domain.Match;

public class Match
{
    public Guid Id { get; set; }
    public Guid User1Id { get; set; }
    public Guid User2Id { get; set; }
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;

    // Compatibility/accessor properties expected by application layer
    public string UserId1 => User1Id.ToString();
    public string UserId2 => User2Id.ToString();
    public DateTime CreatedAt => MatchedAt;

    public bool InvolvesUser(string userId)
    {
        if (Guid.TryParse(userId, out var g))
            return User1Id == g || User2Id == g;
        return User1Id.ToString() == userId || User2Id.ToString() == userId;
    }

    public static Match CreateMutual(string userId1, string userId2, DateTime? createdAt = null)
    {
        var m = new Match();
        if (Guid.TryParse(userId1, out var g1)) m.User1Id = g1; else m.User1Id = Guid.NewGuid();
        if (Guid.TryParse(userId2, out var g2)) m.User2Id = g2; else m.User2Id = Guid.NewGuid();
        m.MatchedAt = createdAt ?? DateTime.UtcNow;
        m.Id = Guid.NewGuid();
        return m;
    }
}
