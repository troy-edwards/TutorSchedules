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
	public List<OutOfCenterDisplayRow> OutOfCenterTutors = new();
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
		var outOfCenterBlocks = await _context.OutOfCenterBlocks.ToListAsync();
		BuildActiveListFromBlocks(fullTutorList, outOfCenterBlocks);
	}

	private void BuildActiveListFromBlocks(List<TimeBlock> fullTutorList, List<OutOfCenterBlock> outOfCenterBlocks)
	{
		// Convert UTC to Central Time
		var centralZone = TimeZoneInfo.FindSystemTimeZoneById("America/Chicago");
		var centralNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, centralZone);
		
		TimeToUse = UseCustomTime ? CustomTime : TimeOnly.FromDateTime(centralNow);
		WeekdayToUse = UseCustomTime ? CustomWeekDay : centralNow.DayOfWeek;
		var dateOnlyToUse = DateOnly.FromDateTime(centralNow.GetNextWeekday(WeekdayToUse)); 
		DateToUse = UseCustomTime ? new DateTime(dateOnlyToUse, TimeToUse) : centralNow;
		//add case for not showing subject

		var scheduleByTutor = fullTutorList.ToLookup(b => b.TutorId);
		var outOfCenterByTutor = outOfCenterBlocks.ToLookup(b => b.TutorId);
		var scheduledBlocks = fullTutorList
			.Where(t => DateToUse.OccursDuring(t.Weekday, t.StartTime, t.EndTime))
			.Select(t => (Block: t, OutOfCenter: CenterPresence.FindOutOfCenterBlock(outOfCenterByTutor[t.TutorId], DateToUse)))
			.ToList();

		ActiveTutors = scheduledBlocks
			.Where(s => s.OutOfCenter is null)
			.Select(s =>
			{
				var (arrival, departure) =
					CenterPresence.GetTimeInCenter(s.Block, outOfCenterByTutor[s.Block.TutorId], TimeToUse);
				return new DashboardDisplayRow
				{
					TutorName = s.Block.Tutor.DisplayName,
					ArrivalDisplay = arrival.ToString(),
					DepartureString = departure.ToString(),
					SubjectConfidence = GetConfidence(s.Block.TutorId)
				};
			})
			.OrderByDescending(r => r.SubjectConfidence)
			.ToList();

		OutOfCenterTutors = scheduledBlocks
			.Where(s => s.OutOfCenter is not null)
			.Select(s =>
			{
				var returnWindow = CenterPresence.FindTimeInCenterAfter(s.OutOfCenter!,
					scheduleByTutor[s.Block.TutorId], outOfCenterByTutor[s.Block.TutorId]);
				return new OutOfCenterDisplayRow
				{
					TutorName = s.Block.Tutor.DisplayName,
					Location = s.OutOfCenter!.Location,
					TimeOutDisplay = $"{s.OutOfCenter.StartTime} - {s.OutOfCenter.EndTime}",
					ReturnDisplay = returnWindow is null
						? "No"
						: $"{returnWindow.Value.Arrival} - {returnWindow.Value.Departure}",
					SubjectConfidence = GetConfidence(s.Block.TutorId)
				};
			})
			.OrderByDescending(r => r.SubjectConfidence)
			.ToList();
	}

	private int? GetConfidence(int tutorId)
	{
		if (!ShowSubject)
			return null;
		return Subject?.TutorConfidences.FirstOrDefault(s => s.TutorId == tutorId)?.ConfidenceLevel;
	}
}