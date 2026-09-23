using TutorSchedules.Models;
using TutorSchedules.Utilities;

namespace UnitTests;

[TestFixture]
public class CenterPresenceTest
{
    private readonly TimeOnly _tenAm = new TimeOnly(10, 0);
    private readonly TimeOnly _noon = new TimeOnly(12, 0);
    private readonly TimeOnly _twelveThirty = new TimeOnly(12, 30);
    private readonly TimeOnly _onePm = new TimeOnly(13, 0);
    private readonly TimeOnly _twoPm = new TimeOnly(14, 0);
    private readonly TimeOnly _threePm = new TimeOnly(15, 0);
    private readonly TimeOnly _fourPm = new TimeOnly(16, 0);
    private readonly TimeOnly _fivePm = new TimeOnly(17, 0);
    private readonly TimeOnly _sixPm = new TimeOnly(18, 0);
    private readonly TimeOnly _sevenPm = new TimeOnly(19, 0);

    // Master schedule: Monday 1 PM - 6 PM.
    private TimeBlock _mondayShift;

    [SetUp]
    public void SetUp()
    {
        _mondayShift = new TimeBlock(1, DayOfWeek.Monday, _onePm, _sixPm);
    }

    private static DateTime MondayAt(int hour, int minute = 0) => new DateTime(2025, 3, 24, hour, minute, 0);

    private static OutOfCenterBlock OutBlock(DayOfWeek weekday, TimeOnly start, TimeOnly end) => new OutOfCenterBlock
    {
        TutorId = 1,
        Weekday = weekday,
        StartTime = start,
        EndTime = end,
        Location = "Music building"
    };

    [Test]
    public void FindOutOfCenterBlock_NoBlocksReturnsNull()
    {
        Assert.That(CenterPresence.FindOutOfCenterBlock(new List<OutOfCenterBlock>(), MondayAt(14, 30)), Is.Null);
    }

    [Test]
    public void FindOutOfCenterBlock_TimeDuringBlockReturnsBlock()
    {
        var block = OutBlock(DayOfWeek.Monday, _twoPm, _threePm);

        Assert.That(CenterPresence.FindOutOfCenterBlock([block], MondayAt(14, 30)), Is.SameAs(block));
    }

    [Test]
    public void FindOutOfCenterBlock_OutAtStartTimeButBackAtEndTime()
    {
        var block = OutBlock(DayOfWeek.Monday, _twoPm, _threePm);

        Assert.That(CenterPresence.FindOutOfCenterBlock([block], MondayAt(14)), Is.SameAs(block));
        Assert.That(CenterPresence.FindOutOfCenterBlock([block], MondayAt(15)), Is.Null);
    }

    [Test]
    public void FindOutOfCenterBlock_BlockOnDifferentDayReturnsNull()
    {
        var block = OutBlock(DayOfWeek.Tuesday, _twoPm, _threePm);

        Assert.That(CenterPresence.FindOutOfCenterBlock([block], MondayAt(14, 30)), Is.Null);
    }

    [Test]
    public void GetTimeInCenter_NoBlocksGivesWholeShift()
    {
        var (arrival, departure) =
            CenterPresence.GetTimeInCenter(_mondayShift, new List<OutOfCenterBlock>(), new TimeOnly(14, 30));

        Assert.That(arrival, Is.EqualTo(_onePm));
        Assert.That(departure, Is.EqualTo(_sixPm));
    }

    [Test]
    public void GetTimeInCenter_BeforeBlockDepartsWhenBlockStarts()
    {
        List<OutOfCenterBlock> blocks = [OutBlock(DayOfWeek.Monday, _twoPm, _threePm)];

        var (arrival, departure) = CenterPresence.GetTimeInCenter(_mondayShift, blocks, new TimeOnly(13, 30));

        Assert.That(arrival, Is.EqualTo(_onePm));
        Assert.That(departure, Is.EqualTo(_twoPm));
    }

    [Test]
    public void GetTimeInCenter_AfterBlockArrivesWhenBlockEnds()
    {
        List<OutOfCenterBlock> blocks = [OutBlock(DayOfWeek.Monday, _twoPm, _threePm)];

        var (arrival, departure) = CenterPresence.GetTimeInCenter(_mondayShift, blocks, new TimeOnly(15, 30));

        Assert.That(arrival, Is.EqualTo(_threePm));
        Assert.That(departure, Is.EqualTo(_sixPm));
    }

    [Test]
    public void GetTimeInCenter_BetweenTwoBlocks()
    {
        List<OutOfCenterBlock> blocks =
        [
            OutBlock(DayOfWeek.Monday, _twoPm, _threePm),
            OutBlock(DayOfWeek.Monday, _fourPm, _fivePm)
        ];

        var (arrival, departure) = CenterPresence.GetTimeInCenter(_mondayShift, blocks, new TimeOnly(15, 30));

        Assert.That(arrival, Is.EqualTo(_threePm));
        Assert.That(departure, Is.EqualTo(_fourPm));
    }

    [Test]
    public void GetTimeInCenter_BlocksExtendingPastShiftAreClippedToIt()
    {
        List<OutOfCenterBlock> blocks =
        [
            OutBlock(DayOfWeek.Monday, _noon, _twoPm),
            OutBlock(DayOfWeek.Monday, _fivePm, _sevenPm)
        ];

        var (arrival, departure) = CenterPresence.GetTimeInCenter(_mondayShift, blocks, new TimeOnly(15, 0));

        Assert.That(arrival, Is.EqualTo(_twoPm));
        Assert.That(departure, Is.EqualTo(_fivePm));
    }

    [Test]
    public void GetTimeInCenter_BlocksOnOtherDaysAreIgnored()
    {
        List<OutOfCenterBlock> blocks = [OutBlock(DayOfWeek.Tuesday, _twoPm, _threePm)];

        var (arrival, departure) = CenterPresence.GetTimeInCenter(_mondayShift, blocks, new TimeOnly(13, 30));

        Assert.That(arrival, Is.EqualTo(_onePm));
        Assert.That(departure, Is.EqualTo(_sixPm));
    }

    [Test]
    public void FindTimeInCenterAfter_BackUntilEndOfShift()
    {
        var block = OutBlock(DayOfWeek.Monday, _twoPm, _threePm);

        Assert.That(CenterPresence.FindTimeInCenterAfter(block, [_mondayShift], [block]),
            Is.EqualTo((_threePm, _sixPm)));
    }

    [Test]
    public void FindTimeInCenterAfter_BackUntilNextOutOfCenterBlock()
    {
        var first = OutBlock(DayOfWeek.Monday, _twoPm, _threePm);
        var second = OutBlock(DayOfWeek.Monday, _fourPm, _fivePm);

        Assert.That(CenterPresence.FindTimeInCenterAfter(first, [_mondayShift], [first, second]),
            Is.EqualTo((_threePm, _fourPm)));
    }

    [Test]
    public void FindTimeInCenterAfter_NullWhenShiftEndsWhileOut()
    {
        var shift = new TimeBlock(1, DayOfWeek.Monday, _tenAm, _twelveThirty);
        var block = OutBlock(DayOfWeek.Monday, _tenAm, _twelveThirty);

        Assert.That(CenterPresence.FindTimeInCenterAfter(block, [shift], [block]), Is.Null);
    }

    [Test]
    public void FindTimeInCenterAfter_NullWhenBlockRunsPastEndOfShift()
    {
        var shift = new TimeBlock(1, DayOfWeek.Monday, _onePm, _fourPm);
        var block = OutBlock(DayOfWeek.Monday, _threePm, _fivePm);

        Assert.That(CenterPresence.FindTimeInCenterAfter(block, [shift], [block]), Is.Null);
    }

    [Test]
    public void FindTimeInCenterAfter_NullWhenNextShiftStartsAfterABreak()
    {
        var morningShift = new TimeBlock(1, DayOfWeek.Monday, _tenAm, _twelveThirty);
        var afternoonShift = new TimeBlock(1, DayOfWeek.Monday, _twoPm, _fivePm);
        var block = OutBlock(DayOfWeek.Monday, _tenAm, _twelveThirty);

        Assert.That(CenterPresence.FindTimeInCenterAfter(block, [morningShift, afternoonShift], [block]), Is.Null);
    }

    [Test]
    public void FindTimeInCenterAfter_BackWhenNextShiftStartsRightAway()
    {
        var morningShift = new TimeBlock(1, DayOfWeek.Monday, _tenAm, _twelveThirty);
        var afternoonShift = new TimeBlock(1, DayOfWeek.Monday, _twelveThirty, _fivePm);
        var block = OutBlock(DayOfWeek.Monday, _tenAm, _twelveThirty);

        Assert.That(CenterPresence.FindTimeInCenterAfter(block, [morningShift, afternoonShift], [block]),
            Is.EqualTo((_twelveThirty, _fivePm)));
    }

    [Test]
    public void FindTimeInCenterAfter_NullWhenAnotherOutOfCenterBlockFollows()
    {
        var first = OutBlock(DayOfWeek.Monday, _twoPm, _threePm);
        var second = OutBlock(DayOfWeek.Monday, _threePm, _fourPm);

        Assert.That(CenterPresence.FindTimeInCenterAfter(first, [_mondayShift], [first, second]), Is.Null);
    }

    [Test]
    public void FindTimeInCenterAfter_ShiftsOnOtherDaysDontCount()
    {
        var mondayShift = new TimeBlock(1, DayOfWeek.Monday, _tenAm, _twelveThirty);
        var tuesdayShift = new TimeBlock(1, DayOfWeek.Tuesday, _twelveThirty, _fivePm);
        var block = OutBlock(DayOfWeek.Monday, _tenAm, _twelveThirty);

        Assert.That(CenterPresence.FindTimeInCenterAfter(block, [mondayShift, tuesdayShift], [block]), Is.Null);
    }
}
