using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;
using TutorSchedules.Utilities;
using TutorSchedules.Utilities.Extensions;

namespace TutorSchedules.Pages;

public class IndexModel : PageModel
{
	private ScheduleContext _context;
	private readonly ILogger<IndexModel> _logger;
	[BindProperty]
	public string? Subject { get; set; }
	[BindProperty]
	public bool UseCustomTime { get; set; }
	[BindProperty]
	public TimeOnly CustomTime { get; set; }
	[BindProperty]
	public DayOfWeek CustomWeekDay { get; set; }
	public TimeOnly TimeToUse { get; set; }
	public DayOfWeek WeekdayToUse { get; set; }
	public DateTime DateToUse { get; set; }
	public List<DashboardDisplayRow> ActiveTutors;

	public IndexModel(ILogger<IndexModel> logger, ScheduleContext context)
	{
		_logger = logger;
		_context = context;
	}
	
	public async Task OnGetAsync()
	{
		TimeToUse = UseCustomTime ? CustomTime : TimeOnly.FromDateTime(DateTime.Now);
		WeekdayToUse = UseCustomTime ? CustomWeekDay : DateTime.Today.DayOfWeek;
		DateToUse = DateTime.Now.GetNextWeekday(WeekdayToUse);
		var fullTutorList =
			await _context.ScheduleBlocks.Include(b => b.Tutor).ToListAsync();
		ActiveTutors = fullTutorList
			.Where(t => DateToUse.OccursDuring(t.Weekday, t.StartTime, t.EndTime))
			.Select(t => new DashboardDisplayRow
			{
				TutorName = t.Tutor.DisplayName, 
				ArrivalDisplay = t.StartTime.ToString(), 
				DepartureString = t.EndTime.ToString()
			})
			.ToList();


	}
}