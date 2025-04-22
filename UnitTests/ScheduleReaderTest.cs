using TutorSchedules.Utilities;

namespace UnitTests;

[TestFixture]
public class ScheduleReaderTest
{
    private List<string> _testLines;
    private Dictionary<string, int> _testNamesToId;
    private ScheduleReader _scheduleReader;
    
    [SetUp]
    public void Setup()
    {
        _testLines =
        [
            "Test Schedule - FallSpring 12345,,,,,,",
            "Tutor Name,Monday,Tuesday,Wednesday,Thursday,Friday,Saturday",
            "Avocado,1:00 PM - 6:00 PM,1:00 PM - 6:00 PM,2:00 PM - 6:00 PM,1:00 PM - 6:00 PM,Off,Off",
            "Bacon,2:00 PM - 6:00 PM,2:00 PM - 5:00 PM,Off,2:00 PM - 5:00 PM,2:00 PM - 7:00 PM,2:00 PM - 6:00 PM",
            "Cheese,9:00 AM - 2:00 PM,Off,9:00 AM - 2:00 PM,4:00 PM - 8:00 PM,3:00 PM - 8:00 PM,Off"
        ];
        
        _testNamesToId = new Dictionary<string, int>
        {
            { "Avocado", 1 },
            { "Bacon", 2 },
            { "Cheese", 3 },
        };
        
        _scheduleReader = new ScheduleReader(_testLines, _testNamesToId);
    }

    [Test]
    public void ExtractScheduleReturnsAList()
    {
        Assert.That(_scheduleReader.ExtractSchedule() is not null);
    }
    
    [Test]
    public void ExceptionThrownWhenFileIsEmpty()
    {
        _testLines = new List<string>();
        _scheduleReader = new ScheduleReader(_testLines, _testNamesToId);
        Assert.Throws<FormatException>(() => _scheduleReader.ExtractSchedule());
    }

    [Test]
    public void TestDataGives13TimeBlocksWhenProcessed()
    {
        var blocks = _scheduleReader.ExtractSchedule();
        Assert.That(blocks.Count, Is.EqualTo(13));
        Assert.That(blocks.ElementAt(0).StartTime, Is.EqualTo(new TimeOnly(13, 0)));
    }

    [Test]
    public void EmptyRowThrowsException()
    {
        _testLines.Add(string.Empty);
        _scheduleReader = new ScheduleReader(_testLines, _testNamesToId);
        Assert.Throws<FormatException>(() => _scheduleReader.ExtractSchedule());
    }

    [Test]
    public void ARowThatDoesntHave7ColumnsThrowsException()
    {
        _testLines.Add("1, 2, 3, 4");
        _scheduleReader = new ScheduleReader(_testLines, _testNamesToId);
        Assert.Throws<FormatException>(() => _scheduleReader.ExtractSchedule());
    }

    [Test]
    public void TutorOnScheduleButNotInListThrowsException()
    {
        _testNamesToId.Remove("Bacon");
        _scheduleReader = new ScheduleReader(_testLines, _testNamesToId);
        Assert.Throws<FormatException>(() => _scheduleReader.ExtractSchedule());
    }

    [Test]
    public void CellThatHasWrongNumberOfTimesThrowsException()
    {
        var brokenRow3 = "Avocado,1:00 PM,1:00 PM - 6:00 PM,2:00 PM - 6:00 PM,1:00 PM - 6:00 PM,Off,Off";
        _testLines[3] = brokenRow3;
        _scheduleReader = new ScheduleReader(_testLines, _testNamesToId);
        Assert.Throws<FormatException>(() => _scheduleReader.ExtractSchedule());
    }

    [Test]
    public void CellWithIncorrectTimeFormatThrowsException()
    {
        
        var brokenRow3 = "Avocado,Not A Time - 6:00 PM,1:00 PM - 6:00 PM,2:00 PM - 6:00 PM,1:00 PM - 6:00 PM,Off,Off";
        _testLines[3] = brokenRow3;
        _scheduleReader = new ScheduleReader(_testLines, _testNamesToId);
        Assert.Throws<FormatException>(() => _scheduleReader.ExtractSchedule());
    }

    [Test]
    public void IntervalWithEndTimeBeforeStartTimeThrowsException()
    { 
        var brokenRow3 = "Avocado,6:00 PM - 1:00 PM,1:00 PM - 6:00 PM,2:00 PM - 6:00 PM,1:00 PM - 6:00 PM,Off,Off";
        _testLines[3] = brokenRow3;
        _scheduleReader = new ScheduleReader(_testLines, _testNamesToId);
        Assert.Throws<FormatException>(() => _scheduleReader.ExtractSchedule());
    }
}