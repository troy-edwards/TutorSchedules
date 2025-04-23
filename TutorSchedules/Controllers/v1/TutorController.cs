using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TutorSchedules.Data;
using TutorSchedules.Models;

namespace TutorSchedules.Controllers.v1;

[Route("v1/tutors")]
[ApiController]
public class TutorController
{
    private readonly ScheduleContext _context;

    public TutorController(ScheduleContext context)
    {
        _context = context;
    }
    
    [HttpGet]
    public async Task<ActionResult<List<Tutor>>> GetTutorTimeBlocksAsync()
    {
        return await _context.Tutors.ToListAsync();
    }
}