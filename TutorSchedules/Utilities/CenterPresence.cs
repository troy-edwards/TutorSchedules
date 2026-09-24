using TutorSchedules.Models;
using TutorSchedules.Utilities.Extensions;

namespace TutorSchedules.Utilities;

/// <summary>
/// Subtracts out-of-center time from the master schedule.
/// </summary>
public static class CenterPresence
{
    /// <summary>
    /// The out-of-center block the tutor is in at the given moment, or null if they aren't out of the center.
    /// </summary>
    public static OutOfCenterBlock? FindOutOfCenterBlock(IEnumerable<OutOfCenterBlock> outOfCenterBlocks, DateTime dateTime)
    {
        return outOfCenterBlocks
            .Where(b => dateTime.OccursDuring(b.Weekday, b.StartTime, b.EndTime))
            .OrderBy(b => b.StartTime)
            .FirstOrDefault();
    }

    /// <summary>
    /// When the tutor arrived in, and will leave, the center around the given time within a master schedule block.
    /// Assumes the tutor is in the center at that time (see <see cref="FindOutOfCenterBlock"/>).
    /// </summary>
    public static (TimeOnly Arrival, TimeOnly Departure) GetTimeInCenter(TimeBlock block,
        IEnumerable<OutOfCenterBlock> outOfCenterBlocks, TimeOnly time)
    {
        var sameDayBlocks = outOfCenterBlocks.Where(b => b.Weekday == block.Weekday).ToList();

        var arrival = sameDayBlocks
            .Where(b => b.EndTime > block.StartTime && b.EndTime <= time)
            .Select(b => b.EndTime)
            .DefaultIfEmpty(block.StartTime)
            .Max();

        var departure = sameDayBlocks
            .Where(b => b.StartTime < block.EndTime && b.StartTime > time)
            .Select(b => b.StartTime)
            .DefaultIfEmpty(block.EndTime)
            .Min();

        return (arrival, departure);
    }

    /// <summary>
    /// The stretch of time the tutor is back in the center, starting as soon as the given out-of-center block ends.
    /// Null if they aren't back right away: they're off (even if a later shift starts after a break) or out of the
    /// center again at that time.
    /// </summary>
    public static (TimeOnly Arrival, TimeOnly Departure)? FindTimeInCenterAfter(OutOfCenterBlock outOfCenterBlock,
        IEnumerable<TimeBlock> scheduleBlocks, IEnumerable<OutOfCenterBlock> outOfCenterBlocks)
    {
        var weekday = outOfCenterBlock.Weekday;
        var returnTime = outOfCenterBlock.EndTime;
        var sameDayOutOfCenter = outOfCenterBlocks.Where(b => b.Weekday == weekday).ToList();

        var scheduledBlock = scheduleBlocks
            .FirstOrDefault(b => b.Weekday == weekday && Covers(b.StartTime, b.EndTime, returnTime));
        if (scheduledBlock is null || sameDayOutOfCenter.Any(b => Covers(b.StartTime, b.EndTime, returnTime)))
            return null;

        return GetTimeInCenter(scheduledBlock, sameDayOutOfCenter, returnTime);
    }

    /// <summary>
    /// Whether someone in the center until the given departure time is there for the whole appointment.
    /// An appointment that would run past midnight never fits.
    /// </summary>
    public static bool StaysThroughAppointment(TimeOnly departure, TimeOnly appointmentStart, TimeSpan appointmentLength)
    {
        var appointmentEnd = appointmentStart.Add(appointmentLength, out var wrappedDays);
        return wrappedDays == 0 && appointmentEnd <= departure;
    }

    private static bool Covers(TimeOnly start, TimeOnly end, TimeOnly time) => start <= time && time < end;
}
