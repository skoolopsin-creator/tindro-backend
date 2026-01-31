namespace Tindro.Domain.Match;

public class Swipe
{
    public Guid Id { get; set; }
    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
    public bool IsLike { get; set; }
    // Compatibility: store numeric direction value to avoid cross-project enum dependency
    public int DirectionValue { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // convenience constructor used in handlers
    public Swipe() { }

    public Swipe(string fromUserId, string toUserId, object direction, DateTime swipedAt)
    {
        if (Guid.TryParse(fromUserId, out var g1)) FromUserId = g1; else FromUserId = Guid.NewGuid();
        if (Guid.TryParse(toUserId, out var g2)) ToUserId = g2; else ToUserId = Guid.NewGuid();
        // Accept boxed enum or numeric value
        if (direction is System.Enum)
        {
            DirectionValue = Convert.ToInt32(direction);
            var name = direction.ToString();
            IsLike = name == "Like" || name == "SuperLike";
        }
        else
        {
            DirectionValue = Convert.ToInt32(direction);
            IsLike = DirectionValue == 1 || DirectionValue == 2;
        }
        CreatedAt = swipedAt;
        Id = Guid.NewGuid();
    }
}
