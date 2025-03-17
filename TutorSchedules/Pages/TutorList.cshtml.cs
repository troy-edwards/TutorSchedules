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
	public string NewTutorName { get; set; }
	public TutorListModel(ScheduleContext context)
	{
		_context = context;
	}

	
	public async Task OnGetAsync()
	{
		Tutors = await _context.Tutors.ToListAsync();
	}

	public async Task OnPostAsync()
	{
		Tutors = await _context.Tutors.ToListAsync();
		var newTutor = new Tutor()
		{
			DisplayName = NewTutorName
		};
		_context.Tutors.Add(newTutor);
		await _context.SaveChangesAsync();
	}
}