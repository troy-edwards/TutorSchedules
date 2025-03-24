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
    public List<TutorComfortValues> ComfortLevels { get; set; }
    
    
    public async Task OnGet()
    {
        Tutor = await _context.Tutors.FindAsync(TutorId);
        ComfortLevels = await ConfidenceListBuilder.GetConfidenceList(_context, Tutor);
    }
}