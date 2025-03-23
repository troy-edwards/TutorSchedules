using TutorSchedules.Models;

namespace TutorSchedules.Utilities;

public struct DashboardDisplayRow
{
    public string TutorName { get; set; }
    public int? SubjectConfidence { get; set; }
    public string ArrivalDisplay { get; set; }
    public string DepartureString { get; set; }
}