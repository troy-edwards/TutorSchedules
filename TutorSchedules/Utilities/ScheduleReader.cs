using System.Collections;
using TutorSchedules.Models;

namespace TutorSchedules.Utilities;

public class ScheduleReader
{
    private readonly IEnumerable<string> _fileLines;
    private readonly Dictionary<string, int> _nameToId;
    private readonly List<TimeBlock> _blocks;
    private string _previousName;
    
    private static readonly List<string> _offStrings = [
        "Off",
        ""
    ];

    public ScheduleReader(IEnumerable<string> fileLines, Dictionary<string, int> nameToId)
    {
        _fileLines = fileLines;
        _nameToId = nameToId;
        _blocks = new List<TimeBlock>();
    }

    public List<TimeBlock> ExtractSchedule()
    {
        _previousName = "";
        _blocks.Clear();
        if (_fileLines is null || _fileLines.Count() < 2)
        {
            throw new FormatException("The file does not have enough lines to read. (Needs at least a Title and Table Header.)");
        }
        
        for (int i = 2; i < _fileLines.Count(); i++)
        {
            ProcessRow(_fileLines.ElementAt(i), i);
        }
        return _blocks;
    }

    private void ProcessRow(string row, int rowNumber)
    {
        if (row is null)
            throw new FormatException($"A row can not be null. Row {rowNumber + 1} is null.");
        var columns = row.Split(",");
        
        if (columns.Length != 7)
            throw new FormatException($"A row must have 7 columns. Row {rowNumber + 1} has {columns.Length} columns. (Merged columns still count as multiple columns.");
        string name = columns.First();
        name = name == "" ? _previousName : name;
        _previousName = name;
        
        if (!_nameToId.ContainsKey(name))
            throw new FormatException($"{name} is on the schedule, but not in the tutor list. Their display name must match what is on the schedule.");
        int tutorId = _nameToId[name];

        for (DayOfWeek day = DayOfWeek.Monday; day <= DayOfWeek.Saturday; day++)
        {
            int columnNumber = (int)day;
            string timeInterval = columns[columnNumber].Trim();
            
            if(TutorIsOff(timeInterval))
                continue;
            var startAndEndTimes = timeInterval.Split("-").Select(s => s.Trim()).ToArray();
            if (startAndEndTimes.Count() != 2)
                throw new FormatException($"Each cell on the schedule must contain exactly 2 times, seperated by a \"-\". Row {rowNumber + 1}, column {columnNumber + 1} does not meet this requirement.");
            var startTime = GetTimeFromString(startAndEndTimes[0], rowNumber, columnNumber);
            var endTime = GetTimeFromString(startAndEndTimes[1], rowNumber, columnNumber);
            if (endTime < startTime)
                throw new FormatException($"End time is before start time at row {rowNumber + 1}, column {columnNumber + 1}.");
            _blocks.Add(new TimeBlock(tutorId, day, startTime, endTime));
        }
    }

    private bool TutorIsOff(string timeInterval)
    {
        return _offStrings.Contains(timeInterval);
    }

    private TimeOnly GetTimeFromString(string timeString, int row, int column)
    {
        bool parseSuccessful = TimeOnly.TryParse(timeString, out TimeOnly time);
        if (!parseSuccessful)
            throw new FormatException($"Times must be in the format \"XX:XX XM\". \"{timeString}\" at row {row + 1}, column {column + 1} was not read.");
        return time;
    }
}