using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TutorSchedules.Pages;

public class IndexModel : PageModel
{
	private readonly ILogger<IndexModel> _logger;
	[BindProperty]
	public String Subject { get; set; }
	[BindProperty]
	public bool UseCustomTime { get; set; }
	[BindProperty]
	public TimeOnly CustomTime { get; set; }
	[BindProperty]
	public DayOfWeek CustomWeekDay { get; set; }

	public IndexModel(ILogger<IndexModel> logger)
	{
		_logger = logger;
	}

	public void OnGet()
	{
	}
}