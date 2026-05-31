namespace Axpo.CodingChallenge.Utils;

public static class TimeUtils
{
    public const string LocalTimeZone = "Europe/London";
    public static readonly TimeZoneInfo LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(LocalTimeZone);
}