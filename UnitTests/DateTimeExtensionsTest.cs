using TutorSchedules.Models;
using TutorSchedules.Utilities.Extensions;

namespace UnitTests;

[TestFixture]
public class DateTimeExtensionsTest
{
    private DateTime _saturday7Am;

    [SetUp]
    public void SetUp()
    {
        _saturday7Am = new DateTime(2025, 3, 22, 7, 0, 0);
    }
    
    [Test]
    public void OccursDuring_WrongWeekdayReturnsFalse()
    {
        bool answer = _saturday7Am.OccursDuring(
            DayOfWeek.Friday,
            new TimeOnly(6, 0), 
            new TimeOnly(8, 0));

        Assert.False(answer);
    }

    [Test]
    public void OccursDuring_TimeBeforeIntervalReturnsFalse()
    {
        bool answer = _saturday7Am.OccursDuring(
            DayOfWeek.Saturday, 
            new TimeOnly(8, 0), 
            new TimeOnly(9, 0));
        
        Assert.False(answer);
    }

    [Test]
    public void OccursDuring_TimeAfterIntervalReturnsFalse()
    {
        bool answer = _saturday7Am.OccursDuring(
            DayOfWeek.Saturday, 
            new TimeOnly(5, 0), 
            new TimeOnly(6, 0));
        
        Assert.False(answer);
    }

    [Test]
    public void OccursDuring_TimeExactlyAtStartReturnsTrue()
    {
        bool answer = _saturday7Am.OccursDuring(
            DayOfWeek.Saturday, 
            new TimeOnly(7, 0), 
            new TimeOnly(8, 0));
        
        Assert.True(answer);
    }

    [Test]
    public void OccursDuring_TimeExactlyAtEndReturnsFalse()
    {
        bool answer = _saturday7Am.OccursDuring(
            DayOfWeek.Saturday, 
            new TimeOnly(6, 0), 
            new TimeOnly(7, 0));
        
        Assert.False(answer);
    }

    [Test]
    public void BlockStartsWithin_BeforeTimeReturnsFalse()
    {
        TimeBlock block = new TimeBlock(0,
            DayOfWeek.Saturday,
            new TimeOnly(6, 0),
            new TimeOnly(8, 0));
        bool answer = _saturday7Am.BlockStartsWithin(30, block);

        Assert.False(answer);
    }

    [Test]
    public void BlockStartsWithin_MinutesLargerThan12HoursReturnsFalse()
    {
        TimeBlock block = new TimeBlock(0,
            DayOfWeek.Saturday,
            new TimeOnly(6, 0),
            new TimeOnly(8, 0));
        Assert.Throws<ArgumentException>(
            () => _saturday7Am.BlockStartsWithin(60*13, block));
    }

    [Test]
    public void BlockStartsWithin_BlockStartsInMoreThanTimeReturnsFalse()
    {
        TimeBlock block = new TimeBlock(0,
            DayOfWeek.Saturday,
            new TimeOnly(8, 0),
            new TimeOnly(9, 0));
        bool answer = _saturday7Am.BlockStartsWithin(30, block);
        
        Assert.False(answer);
    }

    [Test]
    public void BlockStartsWithin_BlockWithRightTimeOnWrongDayReturnsFalse()
    {
        TimeBlock block = new TimeBlock(0,
            DayOfWeek.Friday,
            new TimeOnly(7, 15),
            new TimeOnly(9, 0));
        bool answer = _saturday7Am.BlockStartsWithin(30, block);
        
        Assert.False(answer);
    }
    
    [Test]
    public void BlockStartsWithin_BlockStartingSoonReturnsTrue()
    {
        TimeBlock block = new TimeBlock(0,
            DayOfWeek.Saturday,
            new TimeOnly(7, 15),
            new TimeOnly(9, 0));
        bool answer = _saturday7Am.BlockStartsWithin(30, block);
        
        Assert.True(answer);
    }

    [Test]
    public void GetNextWeekday_IfTargetDayMatchesOriginalDayReturnOriginal()
    {
        var nextWeekday = _saturday7Am.GetNextWeekday(DayOfWeek.Saturday);
        
        Assert.That(nextWeekday.Date, Is.EqualTo(new DateTime(2025, 3, 22).Date));
    }

    [Test]
    public void GetNextWeekday_WorksWithSaturdayToSundayWrapping()
    {
        var nextWeekday = _saturday7Am.GetNextWeekday(DayOfWeek.Sunday);
        
        Assert.That(nextWeekday.Date, Is.EqualTo(new DateTime(2025, 3, 23).Date));
    }

    [Test]
    public void GetNextWeekday_WorksWithoutAWeekWrapping()
    {
        var sunday = _saturday7Am.AddDays(1);
        var thursday = _saturday7Am.AddDays(5);

        var nextWeekday = sunday.GetNextWeekday(DayOfWeek.Thursday);
        
        Assert.That(nextWeekday, Is.EqualTo(thursday));
    }
}