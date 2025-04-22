using Microsoft.IdentityModel.Tokens;
using TutorSchedules.Models;

namespace TutorSchedules.Utilities;

public class ScheduleReader
{
    private readonly IEnumerable<string> _fileLines;
    private readonly Dictionary<string, int> _nameToId;
    private readonly List<TimeBlock> _blocks;
    private string _previousName;

    private static readonly List<string> OffStrings =
    [
        "Off",
        ""
    ];

    public ScheduleReader(IEnumerable<string> fileLines, Dictionary<string, int> nameToId)
    {
        _fileLines = fileLines;
        _nameToId = nameToId;
        _blocks = new List<TimeBlock>();
        _previousName = "";
    }

    public List<TimeBlock> ExtractSchedule()
    {
        ResetData();
        ConfirmHeaderExists();
        ProcessRows();
        return _blocks;
    }

    private void ResetData()
    {
        _previousName = "";
        _blocks.Clear();
    }

    private void ConfirmHeaderExists()
    {
        if (_fileLines is null || _fileLines.Count() < 2)
            throw new FormatException(
                "The file does not have enough lines to read. (Needs at least a Title and Table Header.)");
    }

    private void ProcessRows()
    {
        for (int i = 2; i < _fileLines.Count(); i++)
            ProcessRow(_fileLines.ElementAt(i), i);
    }


    private void ProcessRow(string row, int rowNumber)
    {
        var columns = SplitColumns(row, rowNumber);
        ConfirmColumnsAreValid(rowNumber, columns);
        var name = GetNameFromColumn(columns);
        var tutorId = GetTutorIdFromName(name);
        ExtractTimesFromColumns(rowNumber, columns, tutorId);
    }

    private static string[] SplitColumns(string row, int rowNumber)
    {
        if (row.IsNullOrEmpty())
            throw new FormatException($"A row can not be null. Row {rowNumber + 1} is null.");
        var columns = row.Split(",");
        return columns;
    }

    private static void ConfirmColumnsAreValid(int rowNumber, string[] columns)
    {
        if (columns.Length != 7)
            throw new FormatException(
                $"A row must have 7 columns. Row {rowNumber + 1} has {columns.Length} columns. (Merged columns still count as multiple columns.");
    }

    private string GetNameFromColumn(string[] columns)
    {
        string name = columns.First();
        name = name == "" ? _previousName : name;
        _previousName = name;
        return name;
    }

    private int GetTutorIdFromName(string name)
    {
        if (!_nameToId.TryGetValue(name, out var tutorId))
            throw new FormatException(
                $"{name} is on the schedule, but not in the tutor list. Their display name must match what is on the schedule.");
        return tutorId;
    }

    private void ExtractTimesFromColumns(int rowNumber, string[] columns, int tutorId)
    {
        for (DayOfWeek day = DayOfWeek.Monday; day <= DayOfWeek.Saturday; day++)
        {
            int columnNumber = (int)day;
            string timeInterval = columns[columnNumber].Trim();

            if (TutorIsOff(timeInterval))
                continue;

            var startAndEndTimes = GetStartAndEndTimesAsStrings(rowNumber, timeInterval, columnNumber);
            var startTime = GetTimeFromString(startAndEndTimes[0], rowNumber, columnNumber);
            var endTime = GetTimeFromString(startAndEndTimes[1], rowNumber, columnNumber);
            ConfirmTimesAreInOrder(rowNumber, endTime, startTime, columnNumber);
            _blocks.Add(new TimeBlock(tutorId, day, startTime, endTime));
        }
    }

    private bool TutorIsOff(string timeInterval)
    {
        return OffStrings.Contains(timeInterval);
    }

    private static string[] GetStartAndEndTimesAsStrings(int rowNumber, string timeInterval, int columnNumber)
    {
        var startAndEndTimes = timeInterval.Split("-").Select(s => s.Trim()).ToArray();
        if (startAndEndTimes.Count() != 2)
            throw new FormatException(
                $"Each cell on the schedule must contain exactly 2 times, seperated by a \"-\". Row {rowNumber + 1}, column {columnNumber + 1} does not meet this requirement.");
        return startAndEndTimes;
    }

    private TimeOnly GetTimeFromString(string timeString, int row, int column)
    {
        bool parseSuccessful = TimeOnly.TryParse(timeString, out TimeOnly time);
        if (!parseSuccessful)
            throw new FormatException(
                $"Times must be in the format \"XX:XX XM\". \"{timeString}\" at row {row + 1}, column {column + 1} was not read.");
        return time;
    }

    private static void ConfirmTimesAreInOrder(int rowNumber, TimeOnly endTime, TimeOnly startTime, int columnNumber)
    {
        if (endTime < startTime)
            throw new FormatException(
                $"End time is before start time at row {rowNumber + 1}, column {columnNumber + 1}.");
    }
}