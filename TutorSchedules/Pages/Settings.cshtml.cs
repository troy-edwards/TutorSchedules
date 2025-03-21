using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;
using TutorSchedules.Utilities;

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
			var nameToId = new Dictionary<string, int>();
			await _context.Tutors.ForEachAsync(t => nameToId.TryAdd(t.DisplayName, t.Id));
			var rows = new List<string>();
			string? row = await reader.ReadLineAsync();
			while (row is not null)
			{
				rows.Add(row);
				row = await reader.ReadLineAsync();
			}
			ScheduleReader scheduleReader = new ScheduleReader(rows, nameToId);
			try
			{
				var blocks= scheduleReader.ExtractSchedule();
				_context.ScheduleBlocks.RemoveRange(_context.ScheduleBlocks);
				foreach (var block in blocks)
				{
					_context.ScheduleBlocks.Add(block);
				}
				
				int tutorsAdded = blocks.GroupBy(b => b.Tutor).Count();
				int blocksAdded = blocks.Count;
				await _context.SaveChangesAsync();
				return RedirectToPage($"/UploadSuccessful", new { TutorsAdded = tutorsAdded, BlocksAdded = blocksAdded});
			}
			catch (FormatException e)
			{
				ModelState.AddModelError("ScheduleFile", e.Message);
				return Page();
			}
			
		}
	}


}