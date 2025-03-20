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

	public TutorInfoModel(ScheduleContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> OnGet()
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
				return RedirectToPage("/TutorNotFound");
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
}
