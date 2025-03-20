using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TutorSchedules;

public class UploadSuccessful : PageModel
{
    public int TutorsAdded { get; set; }
    public int BlocksAdded { get; set; }

    public IActionResult OnGet(int TutorsAdded, int BlocksAdded)
    {
        if (BlocksAdded < 0 || TutorsAdded < 0)
        {
            return RedirectToPage("/Sheaningans");
        }

        this.TutorsAdded = TutorsAdded;
        this.BlocksAdded = BlocksAdded;

        return Page();
    }
}