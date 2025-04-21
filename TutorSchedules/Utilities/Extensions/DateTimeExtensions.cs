using TutorSchedules.Models;

namespace TutorSchedules.Utilities.Extensions;

public static class DateTimeExtensions
{
    public static bool OccursDuring(this DateTime givenDate, DayOfWeek weekday, TimeOnly startTime, TimeOnly endTime)
    {
        if (givenDate.DayOfWeek != weekday)
            return false;
        var givenTime = TimeOnly.FromDateTime(givenDate);
        if (givenTime < startTime)
            return false;
        if (givenTime >= endTime)
            return false;
        return true;
    }

    public static bool BlockStartsWithin(this DateTime givenDateTime, int minutes, TimeBlock timeBlock)
    {
        if (minutes > 60 * 12)
            throw new ArgumentException("Limiting future checking to 12 hours.");
        var givenTime = TimeOnly.FromDateTime(givenDateTime);
        if (givenTime >= timeBlock.StartTime)
            return false;
        var futureTime = TimeOnly.FromDateTime(givenDateTime.AddMinutes(minutes));
        if (futureTime < timeBlock.StartTime)
            return false;
        if (givenDateTime.DayOfWeek != timeBlock.Weekday)
            return false;

        return true;
    }

    public static DateTime GetNextWeekday(this DateTime givenDateTime, DayOfWeek targetDay)
    {
        int difference = targetDay - givenDateTime.DayOfWeek;
        if (difference < 0)
            difference += 7;
        return givenDateTime.AddDays(difference);
    }
}