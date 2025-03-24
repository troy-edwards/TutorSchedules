using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Utilities;

public class ConfidenceListBuilder
{
    public static async Task<List<TutorComfortValues>> GetConfidenceList(ScheduleContext context, Tutor tutor)
    {
        return await context.Confidences
            .Include(tsc => tsc.Subject)
            .Where(tsc => tsc.TutorId == tutor.Id)
            .Select(tsc => new TutorComfortValues
            {
                SubjectId = tsc.SubjectId,
                Subject = tsc.Subject,
                Confidence = tsc.ConfidenceLevel
            })
            .ToListAsync();
    }
}