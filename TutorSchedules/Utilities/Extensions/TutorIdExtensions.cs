using System.Globalization;

namespace TutorSchedules.Utilities.Extensions;

public static class TutorIdExtensions
{
    public static string ToDisplayId(this int tutorId) =>
        tutorId.ToString("D7", CultureInfo.InvariantCulture);
}
