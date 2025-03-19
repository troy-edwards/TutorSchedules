using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Pages;


public class TutorListModel : PageModel
{
	public List<Tutor> Tutors { get; set; }
	private ScheduleContext _context;
	[BindProperty]
	public int NewTutorId { get; set; }
	[BindProperty]
	public string NewTutorName { get; set; }
	public TutorListModel(ScheduleContext context)
	{
		_context = context;
	}

	
	public async Task OnGetAsync()
	{
		Tutors = await _context.Tutors.OrderBy(t => t.DisplayName).ToListAsync();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		Tutors = await _context.Tutors.ToListAsync();
		var newTutor = new Tutor()
		{
			Id = NewTutorId,
			DisplayName = NewTutorName
		};
		if (Tutors.Any(t => t.Id == NewTutorId))
			return RedirectToPage("/Error");
		_context.Tutors.Add(newTutor);
		await _context.SaveChangesAsync();
		return RedirectToPage("/TutorList");
	}

	public async Task<IActionResult> OnPostEditAsync(int idToEdit)
	{
		return RedirectToPage("/Login");
	}
	
	public async Task<IActionResult> OnPostDeleteAsync(int idToDelete)
	{
		Tutors = await _context.Tutors.ToListAsync();
		var tutorToDelete = Tutors.FirstOrDefault(t => t.Id == idToDelete);
		if(tutorToDelete != null)
		{
			_context.Tutors.Remove(tutorToDelete);
			await _context.SaveChangesAsync();
		}

		return RedirectToPage("/TutorList");
	}
}