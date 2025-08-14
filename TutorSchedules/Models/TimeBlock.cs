using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutorSchedules.Models;

public class TimeBlock
{
    public TimeBlock(int tutorId, DayOfWeek weekday, TimeOnly startTime, TimeOnly endTime)
    {
        TutorId = tutorId;
        Weekday = weekday;
        StartTime = startTime;
        EndTime = endTime;
    }

    private TimeOnly MinTime(TimeOnly a, TimeOnly b) => a <= b ? a : b;
    private TimeOnly MaxTime(TimeOnly a, TimeOnly b) => a >= b ? a : b;
    
    
    
    public TimeSpan Overlap(TimeBlock otherBlock)
    {
        if(Weekday != otherBlock.Weekday)
            return TimeSpan.Zero;
        var minEnd = MinTime(EndTime, otherBlock.EndTime);
        var maxStart = MaxTime(StartTime, otherBlock.StartTime);
        return maxStart > minEnd ? TimeSpan.Zero : minEnd - maxStart;
    }
    
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int TutorId { get; set; }
    [JsonIgnore] public Tutor? Tutor { get; set; }

    [Required]
    public DayOfWeek Weekday { get; set; }
    [Required]
    public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
}