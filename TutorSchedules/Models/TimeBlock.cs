using System.ComponentModel.DataAnnotations;

namespace TutorSchedules.Models;

public class TimeBlock
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int TutorId { get; set; }
    public Tutor Tutor { get; set; }

    [Required]
    public DayOfWeek Weekday { get; set; }
    [Required]
    public TimeOnly StartTime { get; set; }
    [Required]
    public TimeOnly EndTime { get; set; }
}