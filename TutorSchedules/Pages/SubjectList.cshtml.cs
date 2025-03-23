using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Pages;

public class SubjectList : PageModel
{
    public List<Subject> Subjects { get; set; }
    [BindProperty]
    public string NewSubjectId { get; set; }
    [BindProperty]
    public string NewSubjectName { get; set; }
    
    
    
    private ScheduleContext _context;
    
    public SubjectList(ScheduleContext context)
    {
        _context = context;
    }

    public async Task OnGetAsync()
    {
        Subjects = await _context.Subjects.OrderBy(s => s.Order).ToListAsync();
    }

    public async Task<IActionResult> OnPostAddSubjectAsync()
    {
        Subjects = await _context.Subjects.ToListAsync();
        if (Subjects.Any(s => s.SubjectId == NewSubjectId))
            return RedirectToPage("/Error");
        var newSubject = new Subject
        {
            SubjectId = NewSubjectId,
            Name = NewSubjectName,
            Order = Subjects.Count + 1
        };
        _context.Subjects.Add(newSubject);
        await _context.SaveChangesAsync();
        return RedirectToPage("/SubjectList");
    }
    
    public async Task<IActionResult> OnPostDeleteAsync(string idToDelete)
    {
        Subjects = await _context.Subjects.ToListAsync();
        var subjectToDelete = Subjects.FirstOrDefault(s => s.SubjectId == idToDelete);
        if (subjectToDelete != null)
        {
            _context.Subjects.Remove(subjectToDelete);
            await _context.SaveChangesAsync();
        }

        return RedirectToPage("/SubjectList");
    }
}