using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Pages;

public class TutorOverlaps : PageModel
{

    [BindProperty(SupportsGet = true)]
    public int TutorId { get; set; }
    public Tutor Tutor { get; set; }
    private ScheduleContext _context;
 
    
    public TutorOverlaps(ScheduleContext context)
    {
        _context = context;
    }
    
    public void OnGet()
    {
        //_context.ScheduleBlocks
    }
}