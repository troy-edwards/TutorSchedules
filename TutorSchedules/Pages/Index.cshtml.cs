using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TutorSchedules.Data;
using TutorSchedules.Models;
using TutorSchedules.Utilities;
using TutorSchedules.Utilities.Extensions;

namespace TutorSchedules.Pages;

public class IndexModel : PageModel
{
	public List<SelectListItem> OpenWeekDays = 
	[
		new SelectListItem(DayOfWeek.Monday.ToString(),DayOfWeek.Monday.ToString()),
		new SelectListItem(DayOfWeek.Tuesday.ToString(),DayOfWeek.Tuesday.ToString()),
		new SelectListItem(DayOfWeek.Wednesday.ToString(),DayOfWeek.Wednesday.ToString()),
		new SelectListItem(DayOfWeek.Thursday.ToString(),DayOfWeek.Thursday.ToString()),
		new SelectListItem(DayOfWeek.Friday.ToString(),DayOfWeek.Friday.ToString()),
		new SelectListItem(DayOfWeek.Saturday.ToString(),DayOfWeek.Saturday.ToString()),
	];
	private ScheduleContext _context;
	[BindProperty(SupportsGet = true)]
	public string? SubjectId { get; set; }
	[BindProperty(SupportsGet = true)]
	public bool UseCustomTime { get; set; }
	[BindProperty(SupportsGet = true)]
	public TimeOnly CustomTime { get; set; }
	[BindProperty(SupportsGet = true)]
	public DayOfWeek CustomWeekDay { get; set; }
	public TimeOnly TimeToUse { get; set; }
	public DayOfWeek WeekdayToUse { get; set; }
	public DateTime DateToUse { get; set; }
	public List<DashboardDisplayRow>? ActiveTutors;
	public bool ShowSubject { get; set; }
	public SelectList? SubjectList { get; set; }
	public Subject? Subject { get; set; }
	public IndexModel(ScheduleContext context)
	{
		_context = context;
		ShowSubject = false;
	}

	public async Task SetupVariables()
	{
		bool useInputSubject = !SubjectId.IsNullOrEmpty();
		var subjectList = await _context.Subjects.Include(s => s.TutorConfidences).OrderBy(s => s.Order).ToListAsync();
		if (useInputSubject)
		{
			Subject = subjectList.Find(s => s.SubjectId == SubjectId);
		}
		else
		{
			Subject = subjectList.FirstOrDefault();
		}

		SubjectList = new SelectList(subjectList, nameof(Models.Subject.SubjectId), nameof(Models.Subject.Name),
			Subject); 
		ShowSubject = Subject is not null;
	}

	public async Task OnGetAsync()
	{
		await SetupVariables();
		await PopulateTutorList();
	}

	public async Task<IActionResult> OnPostAsync()
	{
		await SetupVariables();
		await PopulateTutorList();
		return Page();
	}


	public async Task PopulateTutorList()
	{
		var fullTutorList =
			await _context.ScheduleBlocks.Include(b => b.Tutor).ToListAsync();
		BuildActiveListFromBlocks(fullTutorList);
	}

	private void BuildActiveListFromBlocks(List<TimeBlock> fullTutorList)
	{
		TimeToUse = UseCustomTime ? CustomTime : TimeOnly.FromDateTime(DateTime.Now);
		WeekdayToUse = UseCustomTime ? CustomWeekDay : DateTime.Today.DayOfWeek;
		var dateOnlyToUse = DateOnly.FromDateTime(DateTime.Now.GetNextWeekday(WeekdayToUse)); 
		DateToUse = UseCustomTime ? new DateTime(dateOnlyToUse, TimeToUse) : DateTime.Now;
		//add case for not showing subject

		var confidences = Subject?.TutorConfidences;
			
		ActiveTutors = fullTutorList
			.Where(t => DateToUse.OccursDuring(t.Weekday, t.StartTime, t.EndTime))
			.Select(t => new DashboardDisplayRow
			{
				TutorName = t.Tutor.DisplayName, 
				ArrivalDisplay = t.StartTime.ToString(), 
				DepartureString = t.EndTime.ToString(),
				SubjectConfidence = ShowSubject ? confidences?.FirstOrDefault(s => s.TutorId == t.TutorId)?.ConfidenceLevel : null
			})
			.OrderByDescending(r => r.SubjectConfidence)
			.ToList();
	}
}