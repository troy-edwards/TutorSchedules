using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Controllers;

[Route("tutortimeblocks")]
[ApiController]
public class TutorTimeBlocksController
{
    private readonly ScheduleContext _context;

    public TutorTimeBlocksController(ScheduleContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TimeBlock>>> GetTutorTimeBlocksAsync()
    {
        return await _context.ScheduleBlocks.ToListAsync();
    }
}