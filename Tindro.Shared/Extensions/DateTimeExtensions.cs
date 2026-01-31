namespace Tindro.Shared.Extensions;

public static class DateTimeExtensions
{
    public static bool IsExpired(this DateTime date)
        => date < DateTime.UtcNow;
}
