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
    
    [BindProperty]
    public Tutor Tutor { get; set; }
    
    public List<TutorComfortValues> ComfortValues { get; set; }
    public ICollection<TimeBlock>? ScheduledTimes { get; set; }
    public List<OutOfCenterBlock> OutOfCenterBlocks { get; set; } = new();
    [TempData] public string? OutOfCenterError { get; set; }
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
        OutOfCenterBlocks = await _context.OutOfCenterBlocks
            .Where(b => b.TutorId == TutorId)
            .OrderBy(b => b.Weekday)
            .ThenBy(b => b.StartTime)
            .ToListAsync();
        return Page();
    }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();
            
        var existingTutor = await _context.Tutors.FindAsync(TutorId);
        if (existingTutor is null)
            return RedirectToPage("/TutorNotFound");
            
        existingTutor.DisplayName = Tutor.DisplayName;
        await _context.SaveChangesAsync();

        return RedirectToPage("/TutorDetails", new { TutorId });
    }

    public async Task<IActionResult> OnPostAddOutOfCenterAsync(List<DayOfWeek> weekdays, TimeOnly? startTime,
        TimeOnly? endTime, string? location)
    {
        if (!await _context.Tutors.AnyAsync(t => t.Id == TutorId))
            return RedirectToPage("/DataNotFound");

        location = location?.Trim() ?? "";
        if (weekdays.Count == 0)
            OutOfCenterError = "Choose at least one day.";
        else if (startTime is null || endTime is null)
            OutOfCenterError = "Enter a start and end time.";
        else if (endTime <= startTime)
            OutOfCenterError = "End time must be after start time.";
        else if (location.Length == 0 || location.Length > 50)
            OutOfCenterError = "Location must be between 1 and 50 characters long.";
        else
        {
            foreach (var day in weekdays.Distinct())
            {
                _context.OutOfCenterBlocks.Add(new OutOfCenterBlock
                {
                    TutorId = TutorId,
                    Weekday = day,
                    StartTime = startTime.Value,
                    EndTime = endTime.Value,
                    Location = location
                });
            }
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/TutorDetails", new { TutorId });
    }

    public async Task<IActionResult> OnPostDeleteOutOfCenterAsync(int blockId)
    {
        var block = await _context.OutOfCenterBlocks
            .FirstOrDefaultAsync(b => b.Id == blockId && b.TutorId == TutorId);
        if (block is not null)
        {
            _context.OutOfCenterBlocks.Remove(block);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/TutorDetails", new { TutorId });
    }
}