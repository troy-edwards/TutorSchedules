using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutorSchedules.Models;

/// <summary>
/// A weekly time when a tutor is on the master schedule but working somewhere other than the main center
/// (e.g. embedded in a class). Subtracted from the master schedule when deciding who is in the center.
/// </summary>
public class OutOfCenterBlock
{
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

    [Required]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Location must be between 1 and 50 characters long.")]
    public string Location { get; set; } = "";
}
