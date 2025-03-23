using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Pages;

public class TutorInfoModel : PageModel
{
    private ScheduleContext _context;

    [Range(0, 9999999)]
    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    [BindProperty] public Tutor Tutor { get; set; }
    
    public TutorInfoModel(ScheduleContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (Id is null)
        {
            Tutor = new Tutor();
        }
        else
        {
            Tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.Id == Id);
            if (Tutor is null)
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
        if (Id is null)
        {
            if (_context.Tutors.Any(t => t.Id == Tutor.Id))
            {
                ModelState.AddModelError("Tutor.Id", "A tutor with that Id number already exists.");
                return Page();
            }

            await AddNewConfidencesToTutor();
            _context.Tutors.Add(Tutor);
        }
        else if (Tutor.Id == Id)
        {
            _context.Tutors.Attach(Tutor).State = EntityState.Modified;
        }
        else
        {
            return RedirectToPage("/Shenanigans");
        }

        await _context.SaveChangesAsync();
        return RedirectToPage("/TutorList");
    }

    private async Task AddNewConfidencesToTutor()
    {
        var subjects = await _context.Subjects.ToListAsync();
        var newConfidences = new List<TutorSubjectConfidence>();
        foreach (var subject in subjects)
        {
            newConfidences.Add(new TutorSubjectConfidence
                {
                    ConfidenceLevel = null,
                    SubjectId = subject.SubjectId,
                    TutorId = Tutor.Id
                });
        }
        _context.Confidences.AddRange(newConfidences);
    }
}