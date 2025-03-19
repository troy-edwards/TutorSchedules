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
	[Range(0,9999999)]
	[BindProperty(SupportsGet = true)]
	public int? Id { get; set; }
	[BindProperty]
	public Tutor Tutor { get; set; }
	public bool AddingNewTutor { get; set; }

	public TutorInfoModel(ScheduleContext context)
	{
		_context = context;
	}

	public async Task OnGet()
	{
		AddingNewTutor = !Id.HasValue;
		if (AddingNewTutor)
		{
			Tutor = new Tutor();
		}
		else
		{
			Tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.Id == Id);
		}
	}

	public async Task<IActionResult> OnPostAsync()
	{
		if (Tutor.Id == Id)
		{
			_context.Tutors.Attach(Tutor).State = EntityState.Modified;
		}
		else
		{
			_context.Tutors.Add(Tutor);
		}
		await _context.SaveChangesAsync();
		return RedirectToPage("/TutorList");
	}
}
