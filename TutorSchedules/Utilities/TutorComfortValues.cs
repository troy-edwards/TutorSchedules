using TutorSchedules.Models;

namespace TutorSchedules.Utilities;

public class TutorComfortValues
{
    public required Subject Subject { get; set; }
    public int? Confidence { get; set; }
}