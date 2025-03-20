using TutorSchedules.Models;

public class ScheduleReader
{
    private readonly List<string> _fileLines;

    public ScheduleReader(List<string> fileLines)
    {
        _fileLines = fileLines;
    }

    public List<TimeBlock> ExtractSchedule()
    {
        return new List<TimeBlock>();
    }
}