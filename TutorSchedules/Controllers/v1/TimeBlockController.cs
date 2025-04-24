using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Controllers.v1;

[Route("v1/active-time-blocks")]
[ApiController]
public class TimeBlockController
{
    private readonly ScheduleContext _context;

    public TimeBlockController(ScheduleContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<TimeBlock>>> GetTutorTimeBlocksAsync()
    {
        return await _context.ScheduleBlocks.ToListAsync();
    }
}