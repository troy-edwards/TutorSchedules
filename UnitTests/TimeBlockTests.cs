using TutorSchedules.Models;

namespace UnitTests;

[TestFixture]
public class TimeBlockTests
{
    private TimeOnly _noon = new TimeOnly(12, 0);
    private TimeOnly _onePm = new TimeOnly(13, 0);
    private TimeOnly _twoPm = new TimeOnly(14, 0);
    private TimeOnly _threePm = new TimeOnly(15, 0);
    
    
    [Test]
    public void TimespansCanAdd()
    {
        TimeSpan time1 = new TimeSpan(1, 0, 0);
        TimeSpan time2 = new TimeSpan(2, 30, 0);
        
        TimeSpan totalTime = new TimeSpan(3, 30, 0);
        
        Assert.That(time1 + time2, Is.EqualTo(totalTime));
    }    
    
    [Test]
    public void TimespansCanSubtract()
    {
        TimeSpan time1 = new TimeSpan(1, 0, 0);
        TimeSpan time2 = new TimeSpan(2, 30, 0);
        
        TimeSpan difference = new TimeSpan(1, 30, 0);
        
        Assert.That(time2 - time1, Is.EqualTo(difference));
        Assert.That(time1 - time2, Is.EqualTo(-difference));
    }

    [Test]
    public void BlocksOnDifferentDaysHaveNoOverlap()
    {
        TimeBlock firstBlock = new TimeBlock(1, DayOfWeek.Monday, _noon, _onePm);
        TimeBlock secondBlock = new TimeBlock(1, DayOfWeek.Tuesday, _noon, _onePm);
        
        Assert.That(firstBlock.Overlap(secondBlock), Is.EqualTo(TimeSpan.Zero));
    }    
    
    [Test]
    public void IdenticalBlocksHaveEntireDurationAsOverlap()
    {
        TimeBlock block = new TimeBlock(1, DayOfWeek.Monday, _noon, _onePm);
        
        Assert.That(block.Overlap(block), Is.EqualTo(block.EndTime - block.StartTime));
    }

    [Test]
    public void NoOverlapOnSameDayGives0()
    {
        TimeBlock block1 = new TimeBlock(1, DayOfWeek.Monday, _noon, _onePm);
        TimeBlock block2 = new TimeBlock(1, DayOfWeek.Monday, _twoPm, _threePm);
        
        Assert.That(block1.Overlap(block2), Is.EqualTo(TimeSpan.Zero));
    }
    
    
    
}