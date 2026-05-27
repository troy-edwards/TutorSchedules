using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Pages;

public class CourseComfortLevels : PageModel
{
    private readonly ScheduleContext _context;

    public CourseComfortLevels(ScheduleContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public string? SubjectId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string SortBy { get; set; } = "score";

    public List<SelectListItem> SubjectOptions { get; set; } = new();
    public Subject? SelectedSubject { get; set; }

    // Each entry: (Tutor, confidence level or null)
    public List<(Tutor Tutor, int? Confidence)> TutorConfidences { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var subjects = await _context.Subjects.OrderBy(s => s.Order).ToListAsync();

        SubjectOptions = subjects.Select(s => new SelectListItem
        {
            Value = s.SubjectId,
            Text = $"{s.SubjectId} — {s.Name}",
            Selected = s.SubjectId == SubjectId
        }).ToList();

        // Default to first subject if none selected
        if (SubjectId is null && subjects.Any())
            SubjectId = subjects.First().SubjectId;

        if (SubjectId is not null)
        {
            SelectedSubject = subjects.FirstOrDefault(s => s.SubjectId == SubjectId);

            var tutors = await _context.Tutors.ToListAsync();
            var confidences = await _context.Confidences
                .Where(c => c.SubjectId == SubjectId)
                .ToListAsync();

            var combined = tutors.Select(t =>
            {
                var conf = confidences.FirstOrDefault(c => c.TutorId == t.Id);
                return (t, conf?.ConfidenceLevel);
            });

            TutorConfidences = SortBy == "alpha"
                ? combined.OrderBy(x => x.t.DisplayName).ToList()
                : combined
                    .OrderByDescending(x => x.ConfidenceLevel.HasValue)   // nulls last
                    .ThenByDescending(x => x.ConfidenceLevel)              // highest first
                    .ThenBy(x => x.t.DisplayName)                          // alpha tiebreak
                    .ToList();
        }

        return Page();
    }
}

