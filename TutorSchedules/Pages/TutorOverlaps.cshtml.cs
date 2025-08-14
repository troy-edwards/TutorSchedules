using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Pages;

public struct OverlapDisplay
{
    public Tutor Tutor;
    public TimeSpan Overlap;
}

public class TutorOverlaps : PageModel
{

    [BindProperty(SupportsGet = true)]
    public int TutorId { get; set; }
    public Tutor Tutor { get; set; }
    public List<OverlapDisplay> OverlapDisplays { get; set; }
    
    private ScheduleContext _context;
 
    
    public TutorOverlaps(ScheduleContext context)
    {
        _context = context;
    }
    
    public async Task<IActionResult> OnGetAsync()
    {
        Tutor = await _context.Tutors.Include(t => t.ScheduledTimes).FirstOrDefaultAsync(t => t.Id == TutorId);
        if (Tutor is null)
            return RedirectToPage("/DataNotFound");
        await FindOverlapTimeWithOtherTutors();
        return Page();
    }

    private async Task FindOverlapTimeWithOtherTutors()
    {
        var allBlocks = await _context.ScheduleBlocks.ToListAsync();
        var tutorsBlocks = allBlocks.Where(b => b.TutorId == TutorId).ToList();
        var otherTutorBlocks = allBlocks.Where(b => b.TutorId != TutorId).ToList();
        
        var tutorList = await _context.Tutors.ToListAsync();
        tutorList.Remove(Tutor);
        var overlapById = new Dictionary<int, TimeSpan>();
        
        foreach(var tutor in tutorList)
            overlapById.Add(tutor.Id, TimeSpan.Zero);
        
        foreach (var tutorBlock in tutorsBlocks)
        {
            foreach (var otherTutorBlock in otherTutorBlocks)
            {
                overlapById[otherTutorBlock.TutorId] += tutorBlock.Overlap(otherTutorBlock);
            }
        }
        OverlapDisplays = new List<OverlapDisplay>();
        foreach (var pair in overlapById)
        {
            OverlapDisplays.Add(new OverlapDisplay
            {
                Tutor = tutorList.Find(t => t.Id == pair.Key),
                Overlap = pair.Value
            });
        }

        OverlapDisplays = OverlapDisplays.OrderByDescending(d => d.Overlap).ToList();
    }
}