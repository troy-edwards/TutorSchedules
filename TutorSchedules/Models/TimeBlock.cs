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