using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;
namespace TutorSchedules.Pages;

public class EditSubject : PageModel
{
    private ScheduleContext _context;

    [RegularExpression(@"^[A-Za-z]{4}-\d{4}$", ErrorMessage = "Course number must be in the format: XXXX-####")]
    [BindProperty(SupportsGet = true)]
    public string? CourseId { get; set; }
    [BindProperty]
    public Subject Subject { get; set; }

    public EditSubject(ScheduleContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> OnGetAsync()
    {
        if (CourseId is null)
        {
            Subject = new Subject();
        }
        else
        {
            CourseId = CourseId.ToUpper();
            Subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == CourseId);
            if (Subject is null)
            {
                return RedirectToPage("/DataNotFound");
            }
        }
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return RedirectToPage("/Shenanigans");
        if (CourseId is null)
        {
            if (_context.Subjects.Any(s => s.SubjectId == Subject.SubjectId))
            {
                ModelState.AddModelError("Subject.CourseId", "A subject with that course id already exists.");
                return Page();
            }

            await AddNewConfidencesToSubject();
            _context.Subjects.Add(Subject);

        }
        else if (Subject.SubjectId == CourseId)
        {
            _context.Subjects.Attach(Subject).State = EntityState.Modified;
        }
        else
        {
            RedirectToPage("/Shenanigans");
        }
        
        await _context.SaveChangesAsync();
        return RedirectToPage("/SubjectList");
    }

    private async Task AddNewConfidencesToSubject()
    {
        var tutors = await _context.Tutors.ToListAsync();
        var newConfidences = new List<TutorSubjectConfidence>();
        foreach (var tutor in tutors)
        {
            newConfidences.Add(new TutorSubjectConfidence
                {
                    ConfidenceLevel = null,
                    SubjectId = Subject.SubjectId,
                    TutorId = tutor.Id
                });
        }
        _context.Confidences.AddRange(newConfidences);
    }
}
