using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Pages;

public class SettingsModel : PageModel
{
	[BindProperty]
	public IFormFile ScheduleFile { get; set; }

	private ScheduleContext _context;

	public SettingsModel(ScheduleContext context)
	{
		_context = context;
	}

	public void OnGet()
	{
	}

	public async Task<IActionResult> OnPostUploadScheduleAsync()
	{
		if (ScheduleFile is null)
		{
			ModelState.ClearValidationState("ScheduleFile");
			ModelState.AddModelError("ScheduleFile", "Please upload a file.");
			return Page();
		}
		if (ScheduleFile.ContentType != "text/csv")
		{
			ModelState.AddModelError("ScheduleFile", "The file must be a csv file.");
			return Page();
		}
		using( var reader = new StreamReader(ScheduleFile.OpenReadStream()))
		{
			var rows = new List<string>();
			// add row reading
			ScheduleReader scheduleReader = new ScheduleReader(rows);
			try
			{
				var blocks= scheduleReader.ExtractSchedule();
				blocks.Add(new TimeBlock());
				blocks.Add(new TimeBlock());
				blocks.Add(new TimeBlock());
				foreach (var block in blocks)
				{
					_context.ScheduleBlocks.Add(block);
				}
				
				int tutorsAdded = blocks.GroupBy(b => b.Tutor).Count();
				int blocksAdded = blocks.Count;
				await _context.SaveChangesAsync();
				return RedirectToPage($"/UploadSuccessful", new { TutorsAdded = tutorsAdded, BlocksAdded = blocksAdded});
			}
			catch (Exception e)
			{
				ModelState.AddModelError("/ScheduleFile", e.Message);
				return Page();
			}
			
		}
	}


}