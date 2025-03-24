using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;
using TutorSchedules.Utilities;

namespace TutorSchedules.Pages;

public class EditConfidences : PageModel
{
    private ScheduleContext _context;

    public EditConfidences(ScheduleContext context)
    {
        _context = context;
    }

    [BindProperty(SupportsGet = true)]
    public int TutorId { get; set; }
    public Tutor Tutor { get; set; }
    [BindProperty] public List<TutorComfortValues> ComfortLevels { get; set; }
    
    
    public async Task OnGetAsync()
    {
        Tutor = await _context.Tutors.FindAsync(TutorId);
        ComfortLevels = await ConfidenceListBuilder.GetConfidenceList(_context, Tutor);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Tutor = await _context.Tutors.FindAsync(TutorId);
        var confidences = await _context.Confidences.Where(c => c.TutorId == Tutor.Id).ToListAsync();
        foreach (var comfort in ComfortLevels)
        {
            var confidenceElement = confidences.Find(tsc => tsc.SubjectId == comfort.SubjectId);
            confidenceElement.ConfidenceLevel = comfort.Confidence;
            _context.Confidences.Attach(confidenceElement).State = EntityState.Modified;
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("/TutorDetails", new { TutorId = Tutor.Id });
    }
}