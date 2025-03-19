using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Pages;

public class TutorInfoModel : PageModel
{
	private ScheduleContext _context;
	[BindProperty(SupportsGet = true)]
	public int? Id { get; set; }
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
			
			Tutor = await _context.Tutors.FirstOrDefaultAsync(t => t.Id == Id);
	}
}