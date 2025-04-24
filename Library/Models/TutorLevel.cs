using System.Data.Common;

namespace TutorSchedules.Models;

public class TutorLevel
{
    public int Id { get; set; }
    public int Order { get; set; }
    public string? Name { get; set; }
}