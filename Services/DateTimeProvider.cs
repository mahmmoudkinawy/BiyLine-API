namespace BiyLineApi.Services;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetCurrentDateTimeUtc() => DateTime.UtcNow;
}
