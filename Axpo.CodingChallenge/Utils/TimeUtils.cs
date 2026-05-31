namespace Axpo.CodingChallenge.Utils;

public static class TimeUtils
{
    public const string LocalTimeZone = "Europe/London";
    public static readonly TimeZoneInfo LocalTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(LocalTimeZone);

    public static DateTimeOffset GetFirstHourOfDay(DateTime date)
    {
        // Creating the date object for the date in question, midnight and local TZ
        var dateInWallTimeNoTz = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Local);
        var dateInWallTime = new DateTimeOffset(
            date.Year,
            date.Month,
            date.Day,
            0, 0, 0,
            LocalTimeZoneInfo.GetUtcOffset(dateInWallTimeNoTz));

        // Going to 23:00 of the previous day
        return dateInWallTime.AddHours(-1);
    }

    public static TimeOnly AddHoursRespectingTimezone(DateTimeOffset firstHourOfDay, int hoursToAdd)
    {
        var currentHour = firstHourOfDay.AddHours(hoursToAdd);
        var currentHourInCorrectTz = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentHour, LocalTimeZone);
        return TimeOnly.FromDateTime(currentHourInCorrectTz.DateTime);
    }
}