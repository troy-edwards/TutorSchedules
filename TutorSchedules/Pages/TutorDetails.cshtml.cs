using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;
using TutorSchedules.Utilities;

namespace TutorSchedules.Pages;

public class TutorDetails : PageModel
{
    [BindProperty(SupportsGet = true)]
    public int TutorId { get; set; }
    public Tutor Tutor { get; set; }
    public List<TutorComfortValues> ComfortValues { get; set; }
    public ICollection<TimeBlock>? ScheduledTimes { get; set; }
    private ScheduleContext _context;

    public TutorDetails(ScheduleContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> OnGetAsync()
    {
        Tutor = await _context.Tutors.Include(t => t.ScheduledTimes).FirstOrDefaultAsync(t => t.Id == TutorId);
        if (Tutor is null)
            return RedirectToPage("/TutorNotFound");
        ScheduledTimes = Tutor.ScheduledTimes;
        ComfortValues = await ConfidenceListBuilder.GetConfidenceList(_context, Tutor);
        return Page();
    }
}